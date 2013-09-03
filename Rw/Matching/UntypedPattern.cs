using System;

namespace Rw.Matching
{
    public class UntypedPattern : Pattern
    {
        public UntypedPattern()
        {

        }

        public override bool Matches(Expression exp, Environment env)
        {
            return true;
        }
    }
}

