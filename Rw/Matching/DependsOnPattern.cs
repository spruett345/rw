using System;

namespace Rw.Matching
{
    /// <summary>
    /// A lookahead pattern for an expression which
    /// depends on another, or has that symbol in its
    /// set of free variables.
    /// </summary>
    public class DependsOnPattern : Pattern
    {
        private readonly string DependsOn;

        public DependsOnPattern(string depends)
        {
            DependsOn = depends;
        }

        public override bool Matches(Expression exp, MatchEnvironment env)
        {
            if (!env.ContainsKey(DependsOn))
            {
                return false;
            }
            Symbol sym = env[DependsOn] as Symbol;
            if (sym == null)
            {
                return false;
            }

            return exp.FreeVariables().Contains(sym);
        }
        public override bool RequiresLookahead()
        {
            return true;
        }
    }
}

