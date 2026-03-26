import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import Book from '../../Domain/Model/Book'
import type { IBookRepository } from '../../Domain/Services/IBookRepository'
import { HttpBookRepository } from '../../Infrastructure/HttpBookRepository'
import CreateBookCommandHandler from '../../Application/Query/CreateBookCommandHandler'
import { isValidISBN } from '../../Domain/Services/ISBNChecker'
import './book.css'

function AddBook() {

    const [newBook, setNewBook] = useState({ ISBN: '', title: '', author: '', publishDate: '', salesCounter: 0 });
    const [isbnValid, setIsbnValid] = useState<boolean>(false);
    const [isbnTouched, setIsbnTouched] = useState<boolean>(false);
    const navigate = useNavigate();

    const onNewIsbnChange = (v: string) => {
        setNewBook(b => ({ ...b, ISBN: v }));
        setIsbnTouched(true);
        setIsbnValid(isValidISBN(v));
    }

    const addNewBook = async () => {
        if (!isbnValid) {
            window.alert('ISBN inválido. Corrija antes de enviar.');
            return;
        }

        try {
            const bookRepository: IBookRepository = new HttpBookRepository();
            const createHandler = new CreateBookCommandHandler(bookRepository);
            const created = new Book(newBook.ISBN, newBook.title, newBook.author, newBook.publishDate, Number(newBook.salesCounter));
            await createHandler.Handle(created);
            setNewBook({ ISBN: '', title: '', author: '', publishDate: '', salesCounter: 0 });
            setIsbnTouched(false);
            setIsbnValid(false);
            // Navigate back to the books list so it can refresh
            navigate('/books');
        } catch (err: any) {
            console.error('Create failed', err);
            window.alert('Error al crear el libro: ' + (err?.message ?? err));
        }
    }

    return (
        <div className="add-page">
            <div className="add-panel open" id="add-panel">
                <div className="add-panel-inner">
                    <h1>Add a new book</h1>
                    <label className="field">ISBN:
                        <input value={newBook.ISBN} onChange={e => onNewIsbnChange(e.target.value)} onBlur={() => setIsbnTouched(true)} />
                    </label>
                    {!isbnValid && isbnTouched && <div className="isbn-error">ISBN inválido. Use ISBN-10 o ISBN-13.</div>}
                    <label className="field">Title:
                        <input value={newBook.title} onChange={e => setNewBook(b => ({ ...b, title: e.target.value }))} />
                    </label>
                    <label className="field">Author:
                        <input value={newBook.author} onChange={e => setNewBook(b => ({ ...b, author: e.target.value }))} />
                    </label>
                    <label className="field">Publish Date:
                        <input value={newBook.publishDate} onChange={e => setNewBook(b => ({ ...b, publishDate: e.target.value }))} />
                    </label>
                    <label className="field">Sales Counter:
                        <input type="number" value={newBook.salesCounter} onChange={e => setNewBook(b => ({ ...b, salesCounter: Number(e.target.value) }))} />
                    </label>
                    <div className="add-actions">
                        <button className="add-btn" onClick={addNewBook} disabled={!isbnValid || !newBook.title}>Add book</button>
                        <button className="cancel-btn" onClick={() => navigate('/books')}>Cancel</button>
                    </div>
                </div>
            </div>
        </div>
    )

}

export default AddBook
