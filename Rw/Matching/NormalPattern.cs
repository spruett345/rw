using System;

namespace Rw.Matching
{
    public class NormalPattern : Pattern
    {
        private readonly Pattern[] Arguments;
        private readonly string FunctionHead;

        public NormalPattern()
        {
        }
        public override bool Matches(Expression exp, Environment env)
        {
            return false;
        }
    }
}

