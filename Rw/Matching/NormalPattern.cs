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

            if (norm.Head != FunctionHead)
            {
                return false;
            }

            if (norm.Attributes.HasFlag(NormalAttributes.Flat | NormalAttributes.Orderless))
            {
                var state = env.State();
                IEnumerable<Expression> leftover;
                if (MatchesFlatOrderless(norm, env, out leftover))
                {
                    return true;
                }
                env.Revert(state);
                return false;
            }

            return MatchesNormal(norm, env);
        }
        public override bool MatchesPartial(Expression exp, MatchEnvironment env, out Expression matched, out Expression rest)
        {
            Normal norm = exp as Normal;
            if (norm == null || norm.Head != FunctionHead)
            {
                matched = null;
                rest = null;
                return false;
            }
            if (norm.Attributes.HasFlag(NormalAttributes.Flat | NormalAttributes.Orderless))
            {
                IEnumerable<Expression> leftover;
                if (MatchesFlatOrderless(norm, env, out leftover, true))
                {
                    if (leftover.Count() == 0)
                    {
                        matched = exp;
                        rest = null;
                        return true;
                    }
                    List<Expression> notMatched = new List<Expression>(leftover);
                    List<Expression> didMatch = new List<Expression>(norm);
                    for (int i = 0; i < didMatch.Count; i++)
                    {
                        if (notMatched.Contains(didMatch[i]))
                        {
                            notMatched.Remove(didMatch[i]);
                            didMatch.RemoveAt(i);
                            i--;
                        }
                    }
                    matched = didMatch.Count == 1 ? didMatch[0] : norm.Create(didMatch.ToArray());
                    rest = leftover.Count() == 1 ? leftover.First() : norm.Create(leftover.ToArray());

                    return true;
                }
                matched  = null;
                rest = null;
                return false;
            }
            return base.MatchesPartial(exp, env, out matched, out rest);
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

        private bool MatchesFlatOrderless(Normal norm, MatchEnvironment env, out IEnumerable<Expression> leftover, bool partial = false)
        {
            var collector = FindCollector();
            leftover = OrderlessMatching(norm, env, 0, collector);
            if (leftover == null)
            {
                return false;
            }
            if (collector >= 0)
            {
                if (leftover.Count() == 0)
                {
                    return false;
                }
                if (leftover.Count() == 1)
                {
                    Arguments[collector].Bind(leftover.First(), env);
                }
                else
                {
                    Arguments[collector].Bind(norm.Create(leftover.ToArray()), env);
                }
                return true;
            }
            
            return partial || leftover.Count() == 0;
        }
        private IEnumerable<Expression> OrderlessMatching(IEnumerable<Expression> expressions, MatchEnvironment env, int index, int collector)
        {
            if (index == Arguments.Length)
            {
                return expressions;
            }
            if (index == collector)
            {
                return OrderlessMatching(expressions, env, index + 1, collector);
            }
            Pattern pattern = Arguments[index];

            int i = 0;
            foreach (var exp in expressions)
            {
                var state = env.State();
                if (pattern.Matches(exp, env))
                {
                    pattern.Bind(exp, env);
                    var retn = OrderlessMatching(TakeWithout(expressions, i), env, index + 1, collector);
                    if (retn != null)
                    {
                        return retn;
                    }
                }
                env.Revert(state);
                i++;
            }
            return null;
        }
        private IEnumerable<Expression> TakeWithout(IEnumerable<Expression> enumerable, int index)
        {
            int i = 0;
            foreach (var exp in enumerable)
            {
                if (i != index)
                {
                    yield return exp;
                }
                i++;
            }
        }
        private int FindCollector()
        {
            var collector = -1;
            for (int i = 0; i < Arguments.Length; i++)
            {
                Pattern pat = Arguments[i];
                BoundPattern bound = pat as BoundPattern;
                if (bound != null)
                {
                    pat = bound.BasePattern;
                }

                UntypedPattern untyped = pat as UntypedPattern;
                if (untyped != null)
                {
                    collector = i;
                }
                NormalPattern same = pat as NormalPattern;
                if (same != null && same.FunctionHead == FunctionHead)
                {
                    collector = i;
                }
            }
            return collector;
        }

        public override bool RequiresLookahead()
        {
            return Lookahead;
        }
    }
}

