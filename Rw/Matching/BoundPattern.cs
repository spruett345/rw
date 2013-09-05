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

        public override bool Matches(Expression exp, MatchEnvironment env)
        {
            Expression bound = env[Name];
            if (bound != null)
            {
                return exp.Equals(bound) && BasePattern.Matches(exp, env);
            }
            return BasePattern.Matches(exp, env);
        }
        public override void Bind(Expression exp, MatchEnvironment env)
        {
            env.Bind(Name, BasePattern, exp);
        }

        public override bool RequiresLookahead()
        {
            return BasePattern.RequiresLookahead();
        }
        public override bool BindLookahead()
        {
            return BasePattern.BindLookahead();
        }
    }
}

