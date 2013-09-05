using System;
using System.Linq;
using System.Collections.Generic;

namespace Rw.Matching
{
    public class NormalPattern : Pattern
    {
        private readonly Pattern[] Arguments;
        private readonly string FunctionHead;
        private readonly bool Lookahead;

        public NormalPattern(string head, params Pattern[] args)
        {
            FunctionHead = head;
            Arguments = args;

            Lookahead = Arguments.Any((x) => x.RequiresLookahead());
        }

        public override bool Matches(Expression exp, MatchEnvironment env)
        {
            if (exp.Type != TypeClass.Normal)
            {
                return false;
            }

            Normal norm = exp as Normal;
            if (norm.Attributes.HasFlag(NormalAttributes.Flat | NormalAttributes.Orderless))
            {
                return MatchesFlatOrderless(norm, env);
            }

            return MatchesNormal(norm, env);
        }

        private bool MatchesNormal(Normal norm, MatchEnvironment env)
        {
            int index = 0;
            var state = env.State();

            if (norm.Length != Arguments.Length)
            {
                return false;
            }
            if (RequiresLookahead())
            {
                return MatchLookahead(norm, env);
            }
            else
            {
                foreach (var arg in norm)
                {
                    Pattern pat = Arguments[index];

                    if (!pat.Matches(arg, env))
                    {
                        env.Revert(state);
                        return false;
                    }
                    pat.Bind(arg, env);
                    index++;
                }
            }
            return true;
        }

        private bool MatchLookahead(Normal norm, MatchEnvironment env)
        {
            int index = 0;
            var state = env.State();
            foreach (var arg in norm)
            {
                Pattern pat = Arguments[index];
                if (pat.BindLookahead())
                {
                    if (!pat.Matches(arg, env))
                    {
                        env.Revert(state);
                        return false;
                    }
                    pat.Bind(arg, env);
                }
                index++;
            }
            index = 0;
            foreach (var arg in norm)
            {
                Pattern pat = Arguments[index];
                if (!pat.BindLookahead())
                {
                    if (!pat.Matches(arg, env))
                    {
                        env.Revert(state);
                        return false;
                    }
                    pat.Bind(arg, env);
                }
                index++;
            }
            return index == Arguments.Length;
        }

        private bool MatchesFlatOrderless(Normal norm, MatchEnvironment env)
        {
            var collector = -1;
            for (int i = 0; i < Arguments.Length; i++)
            {
                UntypedPattern untyped = Arguments[i] as UntypedPattern;
                if (untyped != null)
                {
                    collector = i;
                }
                NormalPattern same = Arguments[i] as NormalPattern;
                if (same != null && same.FunctionHead == norm.Head)
                {
                    collector = i;
                }
            }
            // TODO: flat/orderless matching
            return false;
        }
        private bool OrderlessMatching(IEnumerable<Expression> expressions, MatchEnvironment env, int index, int collector)
        {
            if (index == Arguments.Length)
            {
                return true;
            }
            if (index == collector)
            {
                return OrderlessMatching(expressions, env, index + 1, collector);
            }
            Pattern pattern = Arguments[index];
            return true;
        }

        public override bool RequiresLookahead()
        {
            return Lookahead;
        }
    }
}

