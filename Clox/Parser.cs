using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clox
{
    public class Parser
    {
        private List<Token> _tokens;
        private int _current = 0;

        public Parser(List<Token> tokens)
        {
            _tokens = tokens;
        }

        public Expr Parse()
        {
            try
            {
                return Expression();
            }
            catch (ParseException)
            {
                return null;
            }
        }

        private Expr Expression()
        {
            return Equality();
        }

        private Expr Equality()
        {
            Expr expr = Comparison();
            
            // Found a != or == and must be parsing an equality expression.
            while (Match(new TokenType[] { TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL }))
            {
                Token @operator = Previous();
                Expr right = Comparison();
                expr = new Expr.Binary(expr, @operator, right);
            }

            return expr;
        }

        private Expr Comparison()
        {
            Expr expr = Term();

            while (Match(new TokenType[] { TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL }))
            {
                Token @operator = Previous();
                Expr right = Term();
                expr = new Expr.Binary(expr, @operator, right);
            }

            return expr;
        }

        private Expr Term()
        {
            Expr expr = Factor();

            while (Match(new TokenType[] { TokenType.MINUS, TokenType.PLUS }))
            {
                Token @operator = Previous();
                Expr right = Factor();
                expr = new Expr.Binary(expr, @operator, right);
            }

            return expr;
        }

        private Expr Factor()
        {
            Expr expr = Unary();

            while (Match(new TokenType[] { TokenType.SLASH, TokenType.STAR }))
            {
                Token @operator = Previous();
                Expr right = Unary();
                expr = new Expr.Binary(expr, @operator, right);
            }

            return expr;
        }

        private Expr Unary()
        {
            if(Match(new TokenType[] { TokenType.BANG, TokenType.MINUS}))
            {
                Token @operator = Previous();
                Expr right = Unary();
                return new Expr.Unary(@operator, right);
            }

            return Primary();
        }

        private Expr Primary()
        {
            if (Match(new TokenType[] { TokenType.FALSE })) return new Expr.Literal(false);
            if (Match(new TokenType[] { TokenType.TRUE })) return new Expr.Literal(true);
            if (Match(new TokenType[] { TokenType.NIL })) return new Expr.Literal(null);

            if (Match(new TokenType[] { TokenType.NUMBER, TokenType.STRING }))
            {
                return new Expr.Literal(Previous().Literal);
            }

            if (Match(new TokenType[] { TokenType.LEFT_PAREN }))
            {
                Expr expr = Expression();
                Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression");
                return new Expr.Grouping(expr);
            }

            throw Error(Peek(), "Expect expression.");
        }

        private void Synchronize()
        {
            Advance();

            while (!IsAtEnd())
            {
                if (Previous().Type == TokenType.SEMICOLON)
                {
                    return;
                }

                switch (Peek().Type)
                {
                    case TokenType.CLASS:
                    case TokenType.FOR:
                    case TokenType.FUN:
                    case TokenType.IF:
                    case TokenType.PRINT:
                    case TokenType.RETURN:
                    case TokenType.VAR:
                    case TokenType.WHILE:
                        return;
                }

                Advance();
            }
        }

        private Token Consume(TokenType type, string message)
        {
            if (Check(type)) return Advance();

            throw Error(Peek(), message);

        }

        private ParseException Error(Token token, string message)
        {
            Clox.Program.Error(token, message);
            return new ParseException();
        }

        private bool Match(IEnumerable<TokenType> types)
        {
            foreach (var type in types)
            {
                if (Check(type))
                {
                    Advance();
                    return true;
                }
            }

            return false;
        }

        private bool Check(TokenType type)
        {
            if (IsAtEnd()) return false;

            return Peek().Type == type;
        }

        private Token Advance()
        {
            if (!IsAtEnd()) _current++;
            return Previous();
        }

        private bool IsAtEnd()
        {
            return Peek().Type == TokenType.EOF;
        }

        private Token Peek()
        {
            return _tokens[_current];
        }

        private Token Previous()
        {
            return _tokens[_current - 1];
        }
    }
}
