using System;

namespace OpenLibrary.Domain.Service;

public static class ISBNChecker
{
    public static bool IsValidISBN(string isbn)
    {
        // Remove any hyphens or spaces
        isbn = isbn.Replace("-", "").Replace(" ", "");

        if (isbn.Length == 10)
        {
            return IsValidISBN10(isbn);
        }
        else if (isbn.Length == 13)
        {
            return IsValidISBN13(isbn);
        }

        return false;
    } 

    private static bool IsValidISBN10(string isbn)
    {
        if (isbn.Length != 10) return false;
        int sum = 0;
        for (int i = 0; i < 9; i++)
        {
            if (!char.IsDigit(isbn[i])) return false;
            sum += (i + 1) * (isbn[i] - '0');
        }

        char last = isbn[9];
        int lastVal;
        if (last == 'X') lastVal = 10;
        else if (char.IsDigit(last)) lastVal = last - '0';
        else return false;

        sum += 10 * lastVal;
        return sum % 11 == 0;
    }

    private static bool IsValidISBN13(string isbn)
    {
        if (isbn.Length != 13) return false;
        int sum = 0;
        for (int i = 0; i < 13; i++)
        {
            if (!char.IsDigit(isbn[i])) return false;
            int digit = isbn[i] - '0';
            sum += digit * (i % 2 == 0 ? 1 : 3);
        }
        return sum % 10 == 0;
    }
}
