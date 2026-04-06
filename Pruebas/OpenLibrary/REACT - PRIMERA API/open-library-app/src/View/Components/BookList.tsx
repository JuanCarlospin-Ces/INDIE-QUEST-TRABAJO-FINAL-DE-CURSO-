import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import Book from '../../Domain/Model/Book'
import type { IBookRepository } from '../../Domain/Services/IBookRepository'
import { HttpBookRepository } from '../../Infrastructure/HttpBookRepository'
import GenericTable from './GenericTable'
import GetAllBooksQueryHandler from '../../Application/Query/GetAllBooksQueryHandler'
import GetMostSoldBooksQueryHandler from '../../Application/Query/GetMostSoldBooksQueryHandler'
import DeleteBookCommandHandler from '../../Application/Query/DeleteBookCommandHandler'
import UpdateBookCommandHandler from '../../Application/Query/UpdateBookCommandHandler'
import './book.css'

function BookList() {

    const [books, setBooks] = useState<Book[]>([])
    const [lightboxSrc, setLightboxSrc] = useState<string | null>(null)

    const fetchBooks = async () => {
        const bookRepository: IBookRepository = new HttpBookRepository();
        const queryHandler = new GetAllBooksQueryHandler(bookRepository);
        const booksData = await queryHandler.Handle();
        setBooks(booksData);
        console.log('booksData', booksData);
    }

    useEffect(() => {
        fetchBooks();
    }, [])

    const [mostSoldBook, setMostSoldBook] = useState<Book | null>(null)
    const fetchMostSoldBook = async () => {
        const bookRepository: IBookRepository = new HttpBookRepository();
        const queryHandler = new GetMostSoldBooksQueryHandler(bookRepository);
        const mostSoldBooksData = await queryHandler.Handle();
        setMostSoldBook(mostSoldBooksData);
        console.log('mostSoldBooksData', mostSoldBooksData);
    }

    useEffect(() => {
        fetchMostSoldBook();
    }, [])

    const [editingISBN, setEditingISBN] = useState<string | null>(null)
    const [editValues, setEditValues] = useState({ title: '', author: '', publishDate: '', salesCounter: 0 })

    // New-book navigation
    const navigate = useNavigate();

    const startEdit = (book: Book) => {
        setEditingISBN(book.ISBN)
        setEditValues({ title: book.title, author: book.author, publishDate: book.publishDate, salesCounter: book.salesCounter })
    }

    

    const cancelEdit = () => {
        setEditingISBN(null)
    }

    const applyUpdate = async (isbn: string) => {
        try {
            const original = books.find(b => b.ISBN === isbn);
            const updated = new Book(isbn, editValues.title, editValues.author, editValues.publishDate, Number(editValues.salesCounter), original?.coverBase64)
            const bookRepository: IBookRepository = new HttpBookRepository();
            const updateHandler = new UpdateBookCommandHandler(bookRepository);
            await updateHandler.Handle(updated);
            setEditingISBN(null);
            await fetchBooks();
            await fetchMostSoldBook();
        } catch (err: any) {
            console.error('Update failed', err);
            window.alert('Error al actualizar el libro: ' + (err?.message ?? err));
        }
    }

    return (

        <>
            <h2 className="section-title">📕 OpenLibrary 📕</h2>

            <div className="add-form">
                <div className="add-actions">
                    <button
                        className="edit-btn"
                        onClick={() => {
                            navigate('/books/add');
                        }}
                        aria-label="Add new book"
                    >
                        ➕ Add new book
                    </button>
                </div>
                {/* Add form moved to a dedicated page. Clicking button navigates to /books/add. */}
            </div>

            <table>
            <thead>
                <tr>
                    <th>Cover</th>
                    <th>ISBN</th>
                    <th>Title</th>
                    <th>Author</th>
                    <th>Publish Date</th>
                    <th>Sales Counter</th>
                </tr>
            </thead>

            <tbody>
                {books.map((book) => (
                    <>
                        <tr key={book.ISBN}>
                            <td className="cover-cell">
                                {book.coverBase64 ? (
                                    <img
                                        src={book.coverBase64.startsWith('data:') ? book.coverBase64 : `data:image/jpeg;base64,${book.coverBase64}`}
                                        alt={`Cover of ${book.title}`}
                                        className="cover-thumb cover-clickable"
                                        onClick={() => setLightboxSrc(book.coverBase64!.startsWith('data:') ? book.coverBase64! : `data:image/jpeg;base64,${book.coverBase64}`)}
                                    />
                                ) : (
                                    <div className="cover-placeholder">Cover Image Not Found</div>
                                )}
                            </td>
                            <td>{book.ISBN}</td>
                            <td>{book.title}</td>
                            <td>{book.author}</td>
                            <td>{book.publishDate}</td>
                            <td className="sales numeric">
                                <div className="sales-wrapper">
                                    <span className="val">{book.salesCounter}</span>
                                    <div className="floating-actions" role="group" aria-label={`Actions for ${book.title}`}>
                                        <button
                                            type="button"
                                            className="edit-btn"
                                            aria-label={`Edit ${book.title}`}
                                            onClick={() => startEdit(book)}
                                        >
                                            ✏️
                                        </button>
                                        <button
                                            type="button"
                                            className="delete-btn"
                                            aria-label={`Delete ${book.title}`}
                                            onClick={async () => {
                                                const confirmed = window.confirm('¿Eliminar libro? Esta acción no se puede deshacer.');
                                                if (!confirmed) return;
                                                try {
                                                    const bookRepository: IBookRepository = new HttpBookRepository();
                                                    const deleteHandler = new DeleteBookCommandHandler(bookRepository);
                                                    await deleteHandler.Handle(book.ISBN);
                                                    await fetchBooks();
                                                    await fetchMostSoldBook();
                                                } catch (err: any) {
                                                    console.error('Delete failed', err);
                                                    window.alert('Error al borrar el libro: ' + (err?.message ?? err));
                                                }
                                            }}
                                        >
                                            🗑️
                                        </button>
                                    </div>
                                </div>
                            </td>
                        </tr>

                        {editingISBN === book.ISBN && (
                            <tr className="edit-row" key={`${book.ISBN}-edit`}>
                                <td colSpan={7}>
                                    <div className="edit-form">
                                        <label>ISBN: <input value={book.ISBN} disabled /></label>
                                        <label>TITLE: <input value={editValues.title} onChange={e => setEditValues(v => ({ ...v, title: e.target.value }))} /></label>
                                        <label>AUTHOR: <input value={editValues.author} onChange={e => setEditValues(v => ({ ...v, author: e.target.value }))} /></label>
                                        <label>PUBLISH DATE: <input value={editValues.publishDate} onChange={e => setEditValues(v => ({ ...v, publishDate: e.target.value }))} /></label>
                                        <label>SALES COUNTER: <input type="number" value={editValues.salesCounter} onChange={e => setEditValues(v => ({ ...v, salesCounter: Number(e.target.value) }))} /></label>
                                        <div className="edit-actions">
                                            <button onClick={() => applyUpdate(book.ISBN)}>Actualizar</button>
                                            <button onClick={cancelEdit}>Cancelar</button>
                                        </div>
                                    </div>
                                </td>
                            </tr>
                        )}
                    </>
                ))}
            </tbody>

            </table>
            
            <h2 className="section-title">Best seller</h2>
            
            <table>
            <thead>
                <tr>
                    <th>Cover</th>
                    <th>ISBN</th>
                    <th>Title</th>
                    <th>Author</th>
                    <th>Publish Date</th>
                    <th>Sales Counter</th>
                </tr>
            </thead>

            <tbody>
                {mostSoldBook && (
                    <tr key={mostSoldBook.ISBN}>
                        <td className="cover-cell">
                            {mostSoldBook.coverBase64 ? (
                                <img
                                    src={mostSoldBook.coverBase64.startsWith('data:') ? mostSoldBook.coverBase64 : `data:image/jpeg;base64,${mostSoldBook.coverBase64}`}
                                    alt={`Cover of ${mostSoldBook.title}`}
                                    className="cover-thumb cover-clickable"
                                    onClick={() => setLightboxSrc(mostSoldBook.coverBase64!.startsWith('data:') ? mostSoldBook.coverBase64! : `data:image/jpeg;base64,${mostSoldBook.coverBase64}`)}
                                />
                            ) : (
                                <div className="cover-placeholder">Cover Image Not Found</div>
                            )}
                        </td>
                        <td>{mostSoldBook.ISBN}</td>
                        <td>{mostSoldBook.title}</td>
                        <td>{mostSoldBook.author}</td>
                        <td>{mostSoldBook.publishDate}</td>
                        <td>{mostSoldBook.salesCounter}</td>
                    </tr>
                )}
            </tbody>

            </table>

            {/* Lightbox modal */}
            {lightboxSrc && (
                <div className="lightbox-overlay" onClick={() => setLightboxSrc(null)}>
                    <img src={lightboxSrc} alt="Cover enlarged" className="lightbox-img" />
                </div>
            )}
        </>
    )

}

export default BookList