using System;
using System.Collections.Generic;
using Rw.Collections;

namespace Rw
{
    public class List : Normal
    {
        public readonly ImmutableList<Expression> Elements;

        public List(Kernel kernel, IEnumerable<Expression> values) : base("list", kernel)
        {
            Elements = new ImmutableList<Expression>(values);
        }
        public List(Kernel kernel, ImmutableList<Expression> list) : base("list", kernel)
        {
            Elements = list;
        }

        public override IEnumerator<Expression> GetEnumerator()
        {           
            return Elements.GetEnumerator();
        }
    }
}

