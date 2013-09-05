using System;

namespace Rw.Matching
{
    public class LiteralPattern : Pattern
    {
        public readonly Expression Literal;

        public LiteralPattern(Expression exp)
        {
            Literal = exp;
        }
        public override bool Matches(Expression exp, MatchEnvironment env)
        {
            return exp.Equals(Literal);
        }
    }
}

