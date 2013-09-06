using System;
using System.Collections.Generic;
using System.Linq;

namespace Rw.Evaluation
{
    public class LookupUnion : Lookup
    {
        private Lookup Left;
        private Lookup Right;

        public LookupUnion(Lookup left, Lookup right)
        {
            Left = left;
            Right = right;
        }
        public override IEnumerable<Rule> ApplicableRules(Expression exp)
        {
            return Left.ApplicableRules(exp).Union(Right.ApplicableRules(exp));
        }

        public override void AddRule(string head, Rule rule)
        {
            throw new NotImplementedException("Unioned lookup is read-only.");
        }

        public override void Clear()
        {
            throw new NotImplementedException("Unioned lookup is read-only.");
        }
        public override void Clear(string head)
        {
            throw new NotImplementedException("Unioned lookup is read-only.");
        }
    }
}

