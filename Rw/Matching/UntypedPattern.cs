using System;

namespace Rw.Matching
{
    /// <summary>
    /// A pattern which will match any expression of any type.
    /// </summary>
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

