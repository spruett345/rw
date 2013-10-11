using System;
using System.Linq;
using System.Collections.Generic;
using Rw.Collections;

namespace Rw
{
    public class List : Normal
    {
        public readonly ImmutableList<Expression> Elements;

        public List(Kernel kernel, IEnumerable<Expression> values) : base("list", kernel)
        {
            if (values.Count() == 0)
            {
                Elements = ImmutableList<Expression>.Empty;
            }
            else
            {
                Elements = new ImmutableList<Expression>(values);
            }
        }
        public List(Kernel kernel, ImmutableList<Expression> list) : base("list", kernel)
        {
            Elements = list;
            ComputedHash = ComputeHash();
        }

        public Expression First()
        {
            return Elements.Value;
        }
        public List Tail()
        {
            if (Elements == ImmutableList<Expression>.Empty)
            {
                return this;
            }
            return new List(Kernel, Elements.Tail);
        }
        public bool Empty()
        {
            return Elements == ImmutableList<Expression>.Empty;
        }

        public override int Length
        {
            get
            {
                return Elements.Size;
            }
        }

        public override Normal Create(params Expression[] args)
        {
            return new List(Kernel, args);
        }
        public List Prepend(Expression expression)
        {
            if (Empty())
            {
                List list = expression as List;
                if (list != null && list.Empty())
                {
                    return this;
                }
            }
            return new List(Kernel, Elements.Prepend(expression));
        }
        public List Map(Expression function)
        {
            return new List(Kernel, Elements.Map((e) => function.Apply(e)));
        }
        public List Filter(Expression function)
        {
            return new List(Kernel, Elements.Filter((e) => {
                var retn = function.Apply(e);
                var b = retn as Boolean;
                if (b != null)
                {
                    return b.Value;
                }
                return false;
            }));
        }
        public Expression Reduce(Expression function, Expression start)
        {
            return Elements.Reduce((x, y) => function.Apply(x, y), start);
        }

        public override IEnumerator<Expression> GetEnumerator()
        {           
            return Elements.GetEnumerator();
        }
        protected override int ComputeHash()
        {
            if (Elements == null)
            {
                return 0;
            }
            int hash = "list".GetHashCode();
            int index = 1;

            foreach (Expression exp in this)
            {
                hash ^= exp.GetHashCode();
            }
            return hash;
        }
    }
}

