using System;

namespace Rw.Matching
{
    /// <summary>
    /// A lookahead pattern which only matches if
    /// the expression is constant with respect to another
    /// bound symbol.
    /// </summary>
    public class ConstantPattern : Pattern
    {
        private readonly string RespectTo;

        public ConstantPattern(string resp)
        {
            RespectTo = resp;
        }

        public override bool RequiresLookahead()
        {
            return true;
        }
        public override bool Matches(Expression exp, MatchEnvironment env)
        {
            if (!env.ContainsKey(RespectTo))
            {
                return false;
            }
            Symbol sym = env[RespectTo] as Symbol;
            if (sym == null)
            {
                return false;
            }

            return !exp.FreeVariables().Contains(sym);
        }
    }
}

