import type Book from "../../Domain/Model/Book";
import type { IBookRepository } from "../../Domain/Services/IBookRepository";

class UpdateBookCommandHandler {
    BookRepository: IBookRepository;

    constructor(bookRepository: IBookRepository) {
        this.BookRepository = bookRepository;
    }

    async Handle(book: Book): Promise<void> {
        await this.BookRepository.updateBook(book);
    }

}

export default UpdateBookCommandHandler;
