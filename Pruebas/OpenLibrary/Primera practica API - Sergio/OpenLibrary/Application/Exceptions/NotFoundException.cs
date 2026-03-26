namespace OpenLibrary.Application.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException() :base("No books found in the library.") { 
        }
    }
}
