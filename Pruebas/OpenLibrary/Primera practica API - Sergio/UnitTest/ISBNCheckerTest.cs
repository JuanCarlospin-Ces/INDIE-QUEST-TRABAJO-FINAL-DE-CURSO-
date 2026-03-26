using OpenLibrary.Domain.Service;
namespace UnitTest
{
    public class ISBNCheckerTest
    {
        [SetUp]
        public void Setup()
        {
        }
        [Test]
        public void TestValidISBN10()
        {
            string isbn = "0-306-40615-2";
            Assert.That(ISBNChecker.IsValidISBN(isbn), Is.True);
        }
        [Test]
        public void TestInvalidISBN10()
        {
            string isbn = "0-306-40615-3";
            Assert.That(ISBNChecker.IsValidISBN(isbn), Is.False);
        }
        [Test]
        public void TestValidISBN13()
        {
            string isbn = "978-3-16-148410-0";
            Assert.That(ISBNChecker.IsValidISBN(isbn), Is.True);
        }
        [Test]
        public void TestInvalidISBN13()
        {
            string isbn = "978-3-16-148410-1";
            Assert.That(ISBNChecker.IsValidISBN(isbn), Is.False);
        }
    }
}
