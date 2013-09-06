using System;
using System.Collections.Generic;
using Rw.Matching;
using Rw.Evaluation;

namespace Rw
{
    public class Kernel
    {
        public Dictionary<string, NormalAttributes> NormalAttributes;

        private Lookup BaseRules;
        private Lookup UserRules;

        public Kernel()
        {
            NormalAttributes = new Dictionary<string, Rw.NormalAttributes>();
            BaseRules = new Lookup();
            UserRules = new Lookup();
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

        public void AddRule(string head, Rule rule)
        {
            BaseRules.AddRule(head, rule);
        }

        public Lookup DefaultRules()
        {
            return BaseRules.Union(UserRules);
        }
    }
}

