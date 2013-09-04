using System;
using System.Collections.Generic;

namespace Rw
{
    public class Kernel
    {
        public Dictionary<string, NormalAttributes> NormalAttributes;

        public Kernel()
        {
            NormalAttributes = new Dictionary<string, Rw.NormalAttributes>();
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

        public Expression Evaluate(Expression exp)
        {
            return exp;
        }
    }
}

