using System;
using OpenLibrary.Domain.Service;

namespace OpenLibrary.Domain.ValueObject;

public class ISBN
{
	public string Value { get; }

	public ISBN(string value)
	{
		if (string.IsNullOrWhiteSpace(value))
			throw new ArgumentException("ISBN cannot be null or empty.", nameof(value));


		if (ISBNChecker.IsValidISBN(value))
		{
			Value = value;
			return;
		}

		throw new ArgumentException("Invalid ISBN format.", nameof(value));
	}

	public override string ToString() => Value;
}
