using System;

namespace Rw.Matching
{
    public class GuardedPattern : Pattern
    {
        private readonly Expression Condition;
        private readonly Pattern BasePattern;

        public GuardedPattern(Expression cond, Pattern pattern)
        {
            Condition = cond;
            BasePattern = pattern;
        }

        public override bool Matches(Expression exp, MatchEnvironment env)
        {
            if (BasePattern.Matches(exp, env))
            {
                var eval = Condition.Substitute(env).Evaluate();
                Boolean b = eval as Boolean;
                if (b != null)
                {
                    return b.Value;
                }
                return false;
            }
            return false;
        }
    }
}

