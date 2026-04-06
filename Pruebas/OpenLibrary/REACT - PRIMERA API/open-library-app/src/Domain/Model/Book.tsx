class Book{
    ISBN : string
    title : string
    author : string
    publishDate : string
    salesCounter : number
    coverBase64? : string

    constructor(ISBN : string, title : string, author : string, publishDate : string, salesCounter : number, coverBase64? : string){
        this.ISBN = ISBN
        this.title = title
        this.author = author
        this.publishDate = publishDate
        this.salesCounter = salesCounter
        this.coverBase64 = coverBase64
    }
}

export default Book