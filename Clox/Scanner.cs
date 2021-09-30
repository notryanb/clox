using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clox
{
    public class Scanner
    {
        private readonly string _source;
        private readonly List<Token> tokens = new List<Token>();
        private int _start = 0;
        private int _current = 0;
        private int _line = 1;

        private static Dictionary<string, TokenType?> Keywords = new Dictionary<string, TokenType?>
        {
            { "and", TokenType.AND },
            { "class", TokenType.CLASS },
            { "else", TokenType.ELSE },
            { "false", TokenType.FALSE },
            { "for", TokenType.FOR },
            { "fun", TokenType.FUN },
            { "if", TokenType.IF },
            { "nil", TokenType.NIL },
            { "or", TokenType.OR },
            { "print", TokenType.PRINT },
            { "return", TokenType.RETURN },
            { "super", TokenType.SUPER },
            { "this", TokenType.THIS },
            { "true", TokenType.TRUE },
            { "var", TokenType.VAR },
            { "while", TokenType.WHILE },
        };

        public Scanner(string source)
        {
            _source = source;
        }

        public List<Token> ScanTokens()
        {
            while(!IsAtEnd())
            {
                _start = _current;
                ScanToken();
            }

            tokens.Add(new Token(TokenType.EOF, string.Empty, null, _line));
            return tokens;
        }

        private bool IsAtEnd()
        {
            return _current >= _source.Length;
        }

        private void ScanToken()
        {
            char c = Advance();
            switch (c)
            {
                case '(': AddToken(TokenType.LEFT_PAREN); break;
                case ')': AddToken(TokenType.RIGHT_PAREN); break;
                case '{': AddToken(TokenType.LEFT_BRACE); break;
                case '}': AddToken(TokenType.RIGHT_BRACE); break;
                case ',': AddToken(TokenType.COMMA); break;
                case '.': AddToken(TokenType.DOT); break;
                case '-': AddToken(TokenType.MINUS); break;
                case '+': AddToken(TokenType.PLUS); break;
                case ';': AddToken(TokenType.SEMICOLON); break;
                case '!':
                    AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
                    break;
                case '=':
                    AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                    break;
                case '<':
                    AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
                    break;
                case '>':
                    AddToken(Match('>') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                    break;
                case '/':
                    if (Match('/'))
                    {
                        // A comment goes until the end of the line.
                        while (Peek() != '\n' && !IsAtEnd()) Advance();
                    }
                    else if (Match('*'))
                    {
                        BlockComment();
                    }
                    else
                    {
                        AddToken(TokenType.SLASH);
                    }
                    break;
                case '*': AddToken(TokenType.STAR); break;
                case ' ':
                case '\r':
                case '\t':
                    // Ignore whitespace
                    break;
                case '\n':
                    _line++;
                    break;
                case '"': String(); break;
                default:
                    if (IsDigit(c))
                    {
                        Number();
                    }
                    else if  (IsAlpha(c))
                    {
                        Identifier();
                    }
                    else
                    {
                        Clox.Program.Error(_line, "Unexpected character.");
                    }
                    break;
            }

        }

        private char Advance()
        {
            return _source[_current++];
        }

        private void AddToken(TokenType type)
        {
            AddToken(type, null);
        }

        private void AddToken(TokenType type, object literal)
        {
            // TODO - See if C# has a better substring where you specify the end index instead of start and length of substring.
            string text = _source.Substring(_start, _current - _start);
            tokens.Add(new Token(type, text, literal, _line));
        }

        private bool Match(char expected)
        {
            if (IsAtEnd()) return false;
            if (_source[_current] != expected) return false;

            _current++;
            return true;
        }

        private char Peek()
        {
            if (IsAtEnd()) return '\0';
            return _source[_current];
        }

        /// <summary>
        /// Consumes a String value and trims the quotes.
        /// </summary>
        private void String()
        {
            while (Peek() != '"' && !IsAtEnd())
            {
                if (Peek() == '\n') _line++;
                Advance();
            }

            if (IsAtEnd())
            {
                Clox.Program.Error(_line, "Unterminated string.");
                return;
            }

            Advance(); // Find the closing "

            // Trim the surrounding ".
            string value = _source.Substring(_start + 1, (_current - 1) - (_start + 1));
            AddToken(TokenType.STRING, value);
        }

        private bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        private void Number()
        {
            while (IsDigit(Peek())) Advance();

            // Look for fractional part of number.
            if (Peek() == '.' && IsDigit(PeekNext()))
            {
                Advance();

                while (IsDigit(Peek())) Advance();
            }

            AddToken(
                TokenType.NUMBER,
                double.Parse(_source.Substring(_start, _current - _start)) // This should never fail because we're sure we're parsing a number.
            );
        }

        /// <summary>
        /// Returns the next value without consuming it or a null char if peeking past the end of source.
        /// </summary>
        /// <returns></returns>
        private char PeekNext()
        {
            if (_current + 1 > _source.Length) return '\0';
            return _source[_current + 1];
        }

        private void BlockComment()
        {
            // Found a block comment, consume everything until the next * is found.
            // Example: /* some code here */
            // Example 2: /* *** Some more Code *** */
            while (!IsAtEnd())
            {
                if (Peek() == '*' && PeekNext() == '/')
                {
                    Advance(); // Consume the closing *
                    Advance(); // Consume the closing /
                    break;
                }

                Advance();
            }

            AddToken(TokenType.BLOCK_COMMENT, _source.Substring(_start, _current - _start));
        }

        private void Identifier()
        {
            while (IsAlphaNumeric(Peek())) Advance();

            string text = _source.Substring(_start, _current - _start);
            TokenType? type;
            Keywords.TryGetValue(text, out type);
            if (type == null) type = TokenType.IDENTIFIER;

            AddToken((TokenType)type);
        }

        private bool IsAlpha(char c)
        {
            return (c >= 'a' && c <= 'z') ||
                   (c >= 'A' && c <= 'Z') ||
                   c == '_';
        }

        private bool IsAlphaNumeric(char c)
        {
            return IsAlpha(c) || IsDigit(c);
        }
    }
}
