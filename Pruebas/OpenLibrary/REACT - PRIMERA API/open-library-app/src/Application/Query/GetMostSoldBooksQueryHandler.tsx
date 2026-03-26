import Book from "../../Domain/Model/Book";
import type { IBookRepository } from "../../Domain/Services/IBookRepository";

class GetMostSoldBooksQueryHandler {
    
    BookRepository : IBookRepository; 

    constructor(bookRepository: IBookRepository) {
        this.BookRepository = bookRepository;
    }

    async Handle (): Promise<Book> {
        return await this.BookRepository.getMostSoldBooks();
    }
}

export default GetMostSoldBooksQueryHandler