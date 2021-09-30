using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clox
{
    public enum TokenType
    {
        // Single token characters
        LEFT_PAREN, RIGHT_PAREN, LEFT_BRACE, RIGHT_BRACE, 
        COMMA, DOT, MINUS, PLUS, SEMICOLON, SLASH, STAR,

        // One or two character tokens
        BANG, BANG_EQUAL, EQUAL, EQUAL_EQUAL,
        GREATER, GREATER_EQUAL, LESS, LESS_EQUAL,

        // Literals
        IDENTIFIER, STRING, NUMBER,

       // Keywords
       AND, CLASS, ELSE, FALSE, RUN, FOR, FUN, IF, NIL, OR,
       PRINT, RETURN, SUPER, THIS, TRUE, VAR, WHILE,

       // Comments
       BLOCK_COMMENT,

       EOF
    }
}
