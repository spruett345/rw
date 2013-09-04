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
            if (Head == "num")
            {
                return exp.Numeric();
            }
            return Head.Equals(exp.Head);
        }

        public override bool BindLookahead()
        {
            return Head == "sym";
        }
    }
}

