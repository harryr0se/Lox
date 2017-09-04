﻿using System.Collections.Generic;

namespace Lox
{
    public class Scanner
    {
        private string source;
        private readonly List<Token> tokens = new List<Token>();
        private int start = 0;
        private int current = 0;
        private int line = 1;
        
        private static readonly Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>
        {
            {"and",         TokenType.AND},
            {"class",       TokenType.CLASS},
            {"else",        TokenType.ELSE},
            {"false",       TokenType.FALSE},
            {"for",         TokenType.FOR},
            {"fun",         TokenType.FUN},
            {"if",          TokenType.IF},
            {"nil",         TokenType.NIL},
            {"or",          TokenType.OR},
            {"print",       TokenType.PRINT},
            {"return",      TokenType.RETURN},
            {"super",       TokenType.SUPER},
            {"this",        TokenType.THIS},
            {"true",        TokenType.TRUE},
            {"var",         TokenType.VAR},
            {"while",       TokenType.WHILE},
            {"break",       TokenType.BREAK},
            {"continue",    TokenType.CONTINUE},
        };
        
        public Scanner(string source) 
        {
            this.source = source;
        }
        
        public List<Token> scanTokens() 
        {
            while (!isAtEnd()) {
                // We are at the beginning of the next lexeme.
                start = current;
                scanToken();
            }

            tokens.Add(new Token(TokenType.EOF, "", null, line));
            return tokens;
        }
        
        private bool isAtEnd() {
            return current >= source.Length;
        }
        
        private void scanToken() 
        {
            char c = advance();
            switch (c) 
            {
                case '(': addToken(TokenType.LEFT_PAREN); break;
                case ')': addToken(TokenType.RIGHT_PAREN); break;
                case '{': addToken(TokenType.LEFT_BRACE); break;
                case '}': addToken(TokenType.RIGHT_BRACE); break;
                case ',': addToken(TokenType.COMMA); break;
                case '.': addToken(TokenType.DOT); break;
                case '-': addToken(TokenType.MINUS); break;
                case '+': addToken(TokenType.PLUS); break;
                case ';': addToken(TokenType.SEMICOLON); break;
                case '*': addToken(TokenType.STAR); break;
                case '=': addToken(match('=') ? TokenType.EQUAL_EQUAL   : TokenType.EQUAL); break;
                case '<': addToken(match('=') ? TokenType.LESS_EQUAL    : TokenType.LESS); break;
                case '!': addToken(match('=') ? TokenType.BANG_EQUAL    : TokenType.BANG); break;
                case '>': addToken(match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER); break;
                case ' ':
                case '\r':
                case '\t':
                    // Ignore whitespace.
                    break;

                case '\n':
                    line++;
                    break;
                case '/':
                    if (match('/')) 
                    {
                        // A comment goes until the end of the line.
                        while (peek() != '\n' && !isAtEnd()) advance();
                    }
                    else if (match('*'))
                    {
                        blockComment();
                    }
                    else
                    {
                        addToken(TokenType.SLASH);
                    }
                    break;
                case '"': String(); break;
                default:
                    if (isDigit(c)) 
                    {
                      number();
                    }
                    else if (isAlpha(c))
                    {
                        identifier();
                    }
                    else
                    {
                        Lox.error(line, "Unexpected character.");
                    }
                    break;
            }
        }

        private void blockComment()
        {
            // Keep track of the level of nesting
            int level = 0;
            
            while (!isAtEnd())
            {
                char c = advance();
                if (c == '*')
                {
                    if (match('/') && 0 == level)
                    {
                        advance();
                        break;
                    }
                    --level;
                }
                else if (c == '/')
                {
                    if (match('*'))
                    {
                        ++level;
                    }
                }
            }
        }

        private static bool isDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        private static bool isAlpha(char c)
        {
            return (c >= 'a' && c <= 'z') ||
                   (c >= 'A' && c <= 'Z') ||
                   (c == '_');
        }
        
        private static bool isAlphaNumeric(char c) 
        {
            return isAlpha(c) || isDigit(c);
        }
        
        private void identifier() 
        {
            while (isAlphaNumeric(peek())) advance();

            // See if the identifier is a reserved word.
            string text = source.Substring(start, current - start);

            TokenType type;
            if (!keywords.TryGetValue(text, out type))
            {
                type = TokenType.IDENTIFIER;
            }
            addToken(type);        
        }

        private void number() 
        {
            while (isDigit(peek())) advance();

            // Look for a fractional part.
            if (peek() == '.' && isDigit(peekNext())) 
            {
                // Consume the "."
                advance();

                while (isDigit(peek())) advance();
            }

            addToken(TokenType.NUMBER,
                double.Parse(source.Substring(start, current - start)));
        }
        
        private char peekNext()
        {
            return current + 1 >= source.Length ? '\0' : source[current + 1];
        } 

        private void String() 
        {
            while (peek() != '"' && !isAtEnd()) {
                if (peek() == '\n') line++;
                advance();
            }

            // Unterminated string.
            if (isAtEnd()) {
                Lox.error(line, "Unterminated string.");
                return;
            }

            // The closing ".
            advance();

            // Trim the surrounding quotes. - 2 because the length of the parse lexeme contains the quotes on either side.
            string value = source.Substring(start + 1, (current - start) - 2);
            addToken(TokenType.STRING, value);
        }
        
        private bool match(char expected) 
        {
            if (isAtEnd()) return false;
            if (source[current] != expected) return false;

            current++;
            return true;
        }
        
        private char peek()
        {
            return isAtEnd() ? '\0' : source[current];
        }
        
        private char advance() 
        {
            return source[current++];
        }

        private void addToken(TokenType type, object literal = null) 
        {
            string text = source.Substring(start, current - start);
            tokens.Add(new Token(type, text, literal, line));
        }
    }
}
