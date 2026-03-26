import type Book from "../../Domain/Model/Book";
import type { IBookRepository } from "../../Domain/Services/IBookRepository";

class CreateBookCommandHandler {
    BookRepository: IBookRepository;

    constructor(bookRepository: IBookRepository) {
        this.BookRepository = bookRepository;
    }

    async Handle(book: Book): Promise<void> {
        await this.BookRepository.createBook(book);
    }

}

export default CreateBookCommandHandler;
