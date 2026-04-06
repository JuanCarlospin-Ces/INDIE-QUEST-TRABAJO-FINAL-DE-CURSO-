import type { IBookRepository } from "../Domain/Services/IBookRepository";
import Book from "../Domain/Model/Book";

export class HttpBookRepository implements IBookRepository {
    async getAllBooks(): Promise<Book[]> {
        const response = await fetch('http://localhost:5246/api/book'); // Insertar URL de la API
        const data = await response.json();

        return (data || []).map((book: any) => {
            // Mapear diferentes formas que pueda devolver la API
            const isbn = book?.isbn?.value ?? book?.ISBN ?? book?.isbn ?? '';
            const title = book?.title ?? '';
            const author = book?.author ?? '';
            // publicationYear puede venir como número; lo convertimos a string para `publishDate`
            const publishDate = book?.publicationYear !== undefined
                ? String(book.publicationYear)
                : (book?.publishDate ?? '');
            const salesCounter = book?.sales ?? book?.salesCounter ?? 0;
            const coverBase64 = book?.coverImageBase64 ?? book?.coverBase64 ?? book?.CoverBase64 ?? undefined;

            return new Book(isbn, title, author, publishDate, salesCounter, coverBase64);
        });
    }

    async getMostSoldBooks(): Promise<Book> {
        const response = await fetch('http://localhost:5246/api/book/most-sold');
        const data = await response.json();
        

        const coverBase64 = data?.coverImageBase64 ?? data?.coverBase64 ?? data?.CoverBase64 ?? undefined;
        return new Book(data.isbn.value, data.title, data.author, data.publicationYear, data.sales, coverBase64);

    }

    async deleteBook(isbn: string): Promise<void> {
        const url = `http://localhost:5246/api/book/${isbn}`;
        const resp = await fetch(url, {
            method: 'DELETE',
        });
        if (!resp.ok) {
            const text = await resp.text().catch(() => '');
            throw new Error(`DELETE ${url} failed: ${resp.status} ${resp.statusText} ${text}`);
        }
    }

    async updateBook(book: Book): Promise<void> {
        const url = `http://localhost:5246/api/book/${book.ISBN}`;
        const resp = await fetch(url, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                isbn: book.ISBN,
                title: book.title,
                author: book.author,
                publicationYear: book.publishDate,
                sales: book.salesCounter,
                coverImageBase64: book.coverBase64 ?? null
            })
        });
        if (!resp.ok) {
            const text = await resp.text().catch(() => '');
            throw new Error(`PUT ${url} failed: ${resp.status} ${resp.statusText} ${text}`);
        }
    }

    async createBook(book: Book): Promise<void> {
        const url = `http://localhost:5246/api/book`;
        const resp = await fetch(url, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                isbn: book.ISBN,
                title: book.title,
                author: book.author,
                publicationYear: book.publishDate,
                sales: book.salesCounter,
                coverImageBase64: book.coverBase64 ?? null
            })
        });
        if (!resp.ok) {
            const text = await resp.text().catch(() => '');
            throw new Error(`POST ${url} failed: ${resp.status} ${resp.statusText} ${text}`);
        }
    }
}