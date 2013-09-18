using System;

namespace Rw.Parsing.Hand
{
    public class Token
    {
        public readonly string Value;
        public readonly TokenType Type;

        public Token(string value, TokenType type)
        {
            Value = value;
            Type = type;
        }
    }
}

