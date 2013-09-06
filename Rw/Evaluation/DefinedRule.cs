using System;
using Rw.Matching;
using System.Collections.Generic;
namespace Rw.Evaluation
{
    public class DefinedRule : Rule
    {
        private readonly Func<Environment, Expression> DefinedCode;

        public DefinedRule(Pattern pattern, Func<Environment, Expression> func) : base(pattern, null)
        {
            DefinedCode = func;
        }

        public override bool Apply(Expression match, out Expression result)
        {
            Expression matched;
            Expression rest;

            MatchEnvironment environment = new MatchEnvironment();
            if (Pattern.MatchesPartial(match, environment, out matched, out rest))
            {
                matched = DefinedCode(environment);
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

