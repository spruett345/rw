using System;

namespace Rw.Parsing.Hand
{
    public enum TokenType
    {
        None = 0,
        Identifier,
        Integer,
        Decimal,
        Symbol,
        Keyword,
        Literal,
        NewLine,
        End
    }
}

