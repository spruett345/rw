using System;

namespace Rw.Matching
{
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

