using System;
using System.Collections.Generic;

namespace Rw.Evaluation
{
    public class Lookup
    {
        private Dictionary<string, List<Rule>> Rules;

        public Lookup()
        {
            Rules = new Dictionary<string, List<Rule>>();
        }

        public Lookup Union(Lookup other)
        {
            return new LookupUnion(this, other);
        }

        public virtual IEnumerable<Rule> ApplicableRules(Expression exp)
        {
            List<Rule> rules;
            if (Rules.TryGetValue(exp.Head, out rules))
            {
                return rules;
            }
            return new Rule[0];
        }

        public virtual void AddRule(string head, Rule rule)
        {
            List<Rule> rules;
            if (Rules.TryGetValue(head, out rules))
            {
                rules.Add(rule);
            }
            else
            {
                rules = new List<Rule>();
                rules.Add(rule);
                Rules.Add(head, rules);
            }
        }
        public virtual void Clear()
        {
            Rules.Clear();
        }
        public virtual void Clear(string head)
        {
            Rules[head] = new List<Rule>();
        }
    }
}

