using System;

namespace Rw.Matching
{
    /// <summary>
    /// Represents a pattern constrained on the type it matches.
    /// This is done first by standard head, or specific defined
    /// types such as 'num'.
    /// </summary>
    public class TypedPattern : Pattern
    {
        private readonly string Head;

        public TypedPattern(string head)
        {
            Head = head;
        }

        public override bool Matches(Expression exp, MatchEnvironment env)
        {
            if (Head == "num")
            {
                return exp.Numeric();
            }
            return Head.Equals(exp.Head);
        }
    }
}

