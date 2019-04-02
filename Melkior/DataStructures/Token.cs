namespace Melkior
{
    public class Token
    {
        public TokenType type;
        public string lexeme;
        public object literal;
        public int line;
        public int column;

        public Token(TokenType type, string lexeme, object literal, int line, int column)
        {
            this.type = type;
            this.lexeme = lexeme;
            this.literal = literal;
            this.line = line;
            this.column = column;
        }

        public override string ToString()
        {
            return lexeme;
        }
    }
}
