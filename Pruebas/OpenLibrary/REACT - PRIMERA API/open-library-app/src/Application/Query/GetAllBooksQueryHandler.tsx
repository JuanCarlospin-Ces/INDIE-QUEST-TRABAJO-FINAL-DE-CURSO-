import type Book from "../../Domain/Model/Book";
import type { IBookRepository } from "../../Domain/Services/IBookRepository";

class GetAllBooksQueryHandler {
    
    BookRepository : IBookRepository;

    constructor(bookRepository: IBookRepository) {
        this.BookRepository = bookRepository;
    }

    async Handle (): Promise<Book[]> {
        return await this.BookRepository.getAllBooks();
    }
    
}

export default GetAllBooksQueryHandler