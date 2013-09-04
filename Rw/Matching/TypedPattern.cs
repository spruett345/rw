using System;

namespace Rw.Matching
{
    public class TypedPattern : Pattern
    {
        private readonly string Head;

        public TypedPattern(string head)
        {
            Head = head;
        }

        public override bool Matches(Expression exp, MatchEnvironment env)
        {
            return Head.Equals(exp.Head);
        }

    }
}

