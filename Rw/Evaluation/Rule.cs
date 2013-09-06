using System;
using Rw.Matching;

namespace Rw.Evaluation
{
    public class Rule
    {
        public readonly Pattern Pattern;
        public readonly Expression Evaluate;

        public Rule(Pattern pattern, Expression exp)
        {
            Pattern = pattern;
            Evaluate = exp;
        }

        public virtual bool Apply(Expression match, out Expression result)
        {
            Expression matched;
            Expression rest;

            MatchEnvironment environment = new MatchEnvironment();
            if (Pattern.MatchesPartial(match, environment, out matched, out rest))
            {
                matched = Evaluate.Substitute(environment);
                if (rest == null)
                {
                    result = matched;
                    return true;
                }
                Normal norm = match as Normal;
                result = norm.Create(matched, rest);
                return true;
            }
            result = null;
            return false;
        }
    }
}

