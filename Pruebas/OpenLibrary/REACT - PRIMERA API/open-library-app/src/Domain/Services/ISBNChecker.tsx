export function isValidISBN(input: string): boolean {
    if (!input) return false;
    let isbn = input.replace(/[-\s]/g, '');

    if (isbn.length === 10) return isValidISBN10(isbn);
    if (isbn.length === 13) return isValidISBN13(isbn);
    return false;
}

function isValidISBN10(isbn: string): boolean {
    if (isbn.length !== 10) return false;
    let sum = 0;
    for (let i = 0; i < 9; i++) {
        const ch = isbn.charAt(i);
        if (ch < '0' || ch > '9') return false;
        sum += (i + 1) * (ch.charCodeAt(0) - '0'.charCodeAt(0));
    }

    const last = isbn.charAt(9);
    let lastVal: number;
    if (last === 'X') lastVal = 10;
    else if (last >= '0' && last <= '9') lastVal = last.charCodeAt(0) - '0'.charCodeAt(0);
    else return false;

    sum += 10 * lastVal;
    return sum % 11 === 0;
}

function isValidISBN13(isbn: string): boolean {
    if (isbn.length !== 13) return false;
    let sum = 0;
    for (let i = 0; i < 13; i++) {
        const ch = isbn.charAt(i);
        if (ch < '0' || ch > '9') return false;
        const digit = ch.charCodeAt(0) - '0'.charCodeAt(0);
        sum += digit * (i % 2 === 0 ? 1 : 3);
    }
    return sum % 10 === 0;
}

export default isValidISBN;
