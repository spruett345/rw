using System;

namespace Rw.Matching
{
    /// <summary>
    /// Abstract base class for all patterns that
    /// are used in pattern matching.
    /// </summary>
    public abstract class Pattern
    {
        public abstract bool Matches(Expression exp, MatchEnvironment env);

        public virtual bool MatchesPartial(Expression exp, MatchEnvironment env, out Expression matched, out Expression rest)
        {
            matched = null;
            rest = null;
            if (Matches(exp, env))
            {
                matched = exp;
                return true;
            }
            return false;
        }
        public virtual void Bind(Expression exp, MatchEnvironment env)
        {

        }

        public virtual bool RequiresLookahead()
        {
            return false;
        }
    }
}

