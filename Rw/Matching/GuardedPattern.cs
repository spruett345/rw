using System;

namespace Rw.Matching
{
    /// <summary>
    /// A pattern with a boolean constraint on it, defined
    /// in a where clause. The pattern will only match if the
    /// base is true and the expression evaluates true
    /// given the match bindings.
    /// </summary>
    public class GuardedPattern : Pattern
    {
        public readonly Expression Condition;
        public readonly Pattern BasePattern;

        public GuardedPattern(Expression cond, Pattern pattern)
        {
            Condition = cond;
            BasePattern = pattern;
        }

        public override bool Matches(Expression exp, MatchEnvironment env)
        {
            if (BasePattern.Matches(exp, env))
            {
                var eval = Condition.Substitute(env).AsImprecise().Evaluate();
                Boolean b = eval as Boolean;
                if (b != null)
                {
                    return b.Value;
                }
                return false;
            }
            return false;
        }
        public override bool MatchesPartial(Expression exp, MatchEnvironment env, out Expression matched, out Expression rest)
        {
            if (BasePattern.MatchesPartial(exp, env, out matched, out rest))
            {
                var eval = Condition.Substitute(env).AsImprecise().Evaluate();
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

