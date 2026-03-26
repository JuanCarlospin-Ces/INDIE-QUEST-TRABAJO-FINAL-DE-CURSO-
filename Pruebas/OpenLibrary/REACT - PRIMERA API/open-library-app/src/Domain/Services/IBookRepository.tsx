import type Book from "../Model/Book";

export interface IBookRepository {
    getAllBooks(): Promise<Book[]>;
    getMostSoldBooks(): Promise<Book>;
    deleteBook(isbn: string): Promise<void>;
    updateBook(book: Book): Promise<void>;
    createBook(book: Book): Promise<void>;
}
