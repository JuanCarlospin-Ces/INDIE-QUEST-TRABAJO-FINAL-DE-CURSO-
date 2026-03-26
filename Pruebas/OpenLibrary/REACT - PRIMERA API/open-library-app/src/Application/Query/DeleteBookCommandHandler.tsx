import type { IBookRepository } from "../../Domain/Services/IBookRepository";

class DeleteBookCommandHandler {

	BookRepository: IBookRepository;

	constructor(bookRepository: IBookRepository) {
		this.BookRepository = bookRepository;
	}

	async Handle(isbn: string): Promise<void> {
		await this.BookRepository.deleteBook(isbn);
	}

}

export default DeleteBookCommandHandler