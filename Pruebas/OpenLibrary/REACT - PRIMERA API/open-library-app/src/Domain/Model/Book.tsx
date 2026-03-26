class Book{
    ISBN : string
    title : string
    author : string
    publishDate : string
    salesCounter : number

    constructor(ISBN : string, title : string, author : string, publishDate : string, salesCounter : number){
        this.ISBN = ISBN
        this.title = title
        this.author = author
        this.publishDate = publishDate
        this.salesCounter = salesCounter
    }
}

export default Book