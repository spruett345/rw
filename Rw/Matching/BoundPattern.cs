using System;

namespace Rw.Matching
{
    public class BoundPattern : Pattern
    {
        public readonly Pattern BasePattern;
        public readonly string Name;

        public BoundPattern(Pattern pattern, string name)
        {
            Name = name;
            BasePattern = pattern;
        }

        public override bool Matches(Expression exp, Environment env)
        {
            Expression bound = env[Name];
            if (bound != null)
            {
                return exp.Equals(bound);
            }
            return BasePattern.Matches(exp, env);
        }
    }
}

