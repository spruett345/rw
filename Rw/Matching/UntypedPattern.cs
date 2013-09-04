using System;

namespace Rw.Matching
{
    public class UntypedPattern : Pattern
    {
        public UntypedPattern()
        {

        }

        public override bool Matches(Expression exp, MatchEnvironment env)
        {
            return true;
        }
    }
}

