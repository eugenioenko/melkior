namespace Melkior
{
    enum TokenType
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

        // one or two character tokens
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
        If,
        In,
        Then,
        Else,
        Elseif,
        Yep,
        Nop,
        Var,
        End,
        Foreach,
        Print,
        Typeof,
        Lambda,
        Func,
        For,
        Return,
        Repeat,
        Until,
        With
    }
}
