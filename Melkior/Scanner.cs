using System;
using System.Collections.Generic;

namespace Melkior
{
    class Scanner
    {
        public string source;
        public List<Token> tokens;
        private int current;
        private int line;
        private int column;
        private int start;

        public Scanner() { }

        private void Init(string source)
        {
            this.source = source;
            current = 0;
            line = 1;
            column = 1;
            start = 0;
            tokens = new List<Token>();
        }

        public List<Token> Scan(string source)
        {
            Init(source);
            try
            {
                while (!Eof())
                {
                    start = current;
                    ScanToken();
                }
                tokens.Add(new Token(TokenType.Eof, null, null, -1, -1));

                return tokens;
            }
            catch (MelkiorError error)
            {
                Console.WriteLine(error);
            }
            catch (Exception)
            {
                Console.WriteLine("[Unhandeled Scan Error] => at " + line + ":" + column);
            }

            return null;
        }

        private bool Eof(){
            return current >= source.Length;
        }

        private char Advance()
        {
            column += 1;
            return source[current++];
        }

        private void AdvanceLine()
        {
            line += 1;
            column = 1;
        }

        private bool Match(char expected)
        {
            if (Eof())
            {
                return false;
            }

            if (source[current] != expected) {
                return false;
            }

            current++;
            return true;
        }

        private char Peek()
        {
            if (Eof())
            {
                return '\0';
            }
            return source[current];
        }

        private char PeekNext()
        {
            if (current + 1 >= source.Length)
            {
                return '\0';
            }
            return source[current + 1];
        }

        private void AddToken(TokenType type, object literal)
        {
            string text = source.Substring(start, current - start);
            var token = new Token(type, text, literal, line, column);
            tokens.Add(token);
        }

        private void Comment()
        {
            while (Peek() != '\n' && !Eof())
            {
                if (Peek() == '\n')
                {
                    AdvanceLine();
                }
                Advance();
            }
        }

        private void String(char quote, TokenType token)
        {
            while (Peek() != quote && !Eof())
            {
                if (Peek() == '\n')
                {
                    AdvanceLine();
                }
                Advance();
            }

            // undeterminated string detected
            if (Eof())
            {
                Error("Undeterminated string, expecting closing quote");
            }

            // the closing quote char
            Advance();

            // Trim the surrounding quotes
            string value = source.Substring(start + 1, current - start - 2);
            AddToken(TokenType.String, value);
        }

        private void Error(string message)
        {
            throw new MelkiorError("[Scan Error]=> (" + line + "): " + message);
        }

        private void Number()
        {

            // the interger part
            while (Char.IsDigit(Peek()))
            {
                Advance();
            }

            // checks for fraction
            if (Peek() == '.' && char.IsDigit(PeekNext()))
            {
                Advance();
            }

            // the decimal part
            while (char.IsDigit(Peek()))
            {
                Advance();
            }

            string value = source.Substring(start, current - start );

            double num = Double.Parse(value);
            AddToken(TokenType.Number, num);

        }

        private void Identifier()
        {
            while (char.IsLetterOrDigit(Peek()) || Peek() == '_')
            {
                Advance();
            }

            var value = source.Substring(start, current - start);
            var capitalized = value[0].ToString().ToUpper() + value.Substring(1).ToLower();
            if (Enum.IsDefined(typeof(TokenType), capitalized))
            {

                TokenType tokenType = (TokenType)Enum.Parse(typeof(TokenType), capitalized);
                if (tokenType != TokenType.String && tokenType != TokenType.Number)
                {
                    AddToken(tokenType, value);
                }
                else
                {
                    AddToken(TokenType.Identifier, value);
                }
            }
            else
            {
                AddToken(TokenType.Identifier, value);
            }
        }

        private void ScanToken()
        {
            char chr = Advance();
            switch (chr)
            {
                case '(': AddToken(TokenType.LeftParen, null); break;
                case ')': AddToken(TokenType.RightParen, null); break;
                case '[': AddToken(TokenType.LeftBracket, null); break;
                case ']': AddToken(TokenType.RightBracket, null); break;
                case '{': AddToken(TokenType.LeftBrace, null); break;
                case '}': AddToken(TokenType.RightBrace, null); break;
                case ',': AddToken(TokenType.Comma, null); break;
                case ':': AddToken(TokenType.Colon, null); break;
                case ';': AddToken(TokenType.Semicolon, null); break;
                case '.': AddToken(TokenType.Dot, null); break;
                case '*': AddToken(Match('=') ? TokenType.StarEqual : TokenType.Star, null); break;
                case '%': AddToken(Match('=') ? TokenType.PercentEqual : TokenType.Percent, null); break;
                case '<': AddToken(Match('=') ? TokenType.LessEqual : TokenType.Less, null); break;
                case '>': AddToken(Match('=') ? TokenType.GreaterEqual : TokenType.Greater, null); break;
                case '!': AddToken(Match('=') ? TokenType.BangEqual : TokenType.Bang, null); break;
                case '=': AddToken(Match('=') ? TokenType.EqualEqual : Match('>') ? TokenType.Arrow : TokenType.Equal, null); break;
                case '-': AddToken(Match('-') ? TokenType.MinusMinus : Match('=') ? TokenType.MinusEqual : TokenType.Minus, null); break;
                case '+': AddToken(Match('+') ? TokenType.PlusPlus : Match('=') ? TokenType.PlusEqual : TokenType.Plus, null); break;
                case '/':
                    if (Match('/'))
                    {
                        Comment();
                    }
                    else if (Match('='))
                    {
                        AddToken(TokenType.SlashEqual, null);
                    }
                    else
                    {
                        AddToken(TokenType.Slash, null);
                    }
                    break;
                case '\n': AdvanceLine(); break;
                case '\'':
                case '"':
                case '`':
                    String(chr, TokenType.String);
                    break;
                // ignore cases
                case ' ':
                case '\r':
                case '\t':
                    break;
                default:
                    if (char.IsDigit(chr))
                    {
                        Number();
                    }
                    else if (char.IsLetter(chr))
                    {
                        Identifier();
                    }
                    else
                    {
                        Error("Unexpected character: " + chr);
                    }
                    break;
            }

        }
    }
}
