namespace Melkior
{
    public enum TokenType
    {
        // parser tokens
        Eof,

        // single character tokens
        LeftBrace,
        LeftBracket,
        LeftParen,
        Percent,
        RightBrace,
        RightBracket,
        RightParen,
        Slash,
        Star,
        Comma,
        Dot,
        Semicolon,

        // one or two character tokens
        Arrow,
        Bang,
        BangEqual,
        Colon,
        Equal,
        EqualEqual,
        Greater,
        GreaterEqual,
        Less,
        LessEqual,
        Minus,
        MinusEqual,
        MinusMinus,
        PercentEqual,
        Plus,
        PlusEqual,
        PlusPlus,
        SlashEqual,
        StarEqual,

        // literals
        Identifier,
        String,
        Null,
        True,
        False,
        Number,
        Regex,

        // keywords
        And,
        Or,
        Not,
        While,
        Do,
        Class,
        If,
        In,
        Then,
        Else,
        Elseif,
        Inherits,
        Yep,
        Nop,
        Var,
        End,
        Base,
        Foreach,
        New,
        Pause,
        Print,
        Typeof,
        Lambda,
        Func,
        For,
        Return,
        Repeat,
        Range,
        Until,
        With
    }
}
