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
    const [coverBase64, setCoverBase64] = useState<string | undefined>(undefined);
    const [coverPreview, setCoverPreview] = useState<string | null>(null);
    const navigate = useNavigate();

    const handleCoverFile = (e: React.ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0];
        if (!file) { setCoverBase64(undefined); setCoverPreview(null); return; }
        const reader = new FileReader();
        reader.onload = () => {
            const dataUrl = reader.result as string;
            setCoverPreview(dataUrl);
            // Extract raw base64 without the data:...;base64, prefix
            const base64 = dataUrl.split(',')[1];
            setCoverBase64(base64);
        };
        reader.readAsDataURL(file);
    };

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
            const created = new Book(newBook.ISBN, newBook.title, newBook.author, newBook.publishDate, Number(newBook.salesCounter), coverBase64);
            await createHandler.Handle(created);
            setNewBook({ ISBN: '', title: '', author: '', publishDate: '', salesCounter: 0 });
            setCoverBase64(undefined);
            setCoverPreview(null);
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
                    <label className="field">Cover Image:
                        <input type="file" accept="image/*" onChange={handleCoverFile} className="file-input" />
                    </label>
                    {coverPreview && <img src={coverPreview} alt="Cover preview" className="cover-preview" />}
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
