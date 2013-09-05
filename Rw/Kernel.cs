using System;
using System.Collections.Generic;
using Rw.Matching;
using Rw.Evaluation;

namespace Rw
{
    public class Kernel
    {
        public Dictionary<string, NormalAttributes> NormalAttributes;

        private Dictionary<string, List<Rule>> RuleLookup;

        public Kernel()
        {
            NormalAttributes = new Dictionary<string, Rw.NormalAttributes>();
            RuleLookup = new Dictionary<string, List<Rule>>();
        }

        public NormalAttributes GetNormalAttributes(string head)
        {
            NormalAttributes val = Rw.NormalAttributes.None;
            if (NormalAttributes.TryGetValue(head, out val))
            {
                return val;
            }
            return Rw.NormalAttributes.None;
        }

        public IEnumerable<Rule> ApplicableRules(Expression exp)
        {
            List<Rule> rules;
            if (RuleLookup.TryGetValue(exp.Head, out rules))
            {
                return rules;
            }
            return new Rule[0];
        }
        public Expression Evaluate(Expression exp)
        {
            return exp;
        }
    }
}

