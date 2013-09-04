using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Rw
{
    public class Normal : Expression, IEnumerable<Expression>
    {
        private readonly string FunctionHead;
        private readonly Expression[] Arguments;

        private readonly int ComputedHash;
        private readonly bool IsNumeric;

        public readonly NormalAttributes Attributes;

        public Normal(string head, Kernel kernel, params Expression[] args) : base(kernel)
        {
            Attributes = Kernel.GetNormalAttributes(head);

            FunctionHead = head;

            if (Attributes.HasFlag(NormalAttributes.Flat))
            {
                Arguments = FlattenArguments(args);
            } 
            else
            {
                Arguments = args;
            }

            if (Attributes.HasFlag(NormalAttributes.Numeric))
            {
                IsNumeric = this.All((x) => x.Numeric());
            } 
            else
            {
                IsNumeric = false;
            }

            ComputedHash = ComputeHash();
            foreach (var arg in Arguments)
            {
                foreach (var sym in arg.FreeVariables())
                {
                   Variables.Add(sym);
                }
            }
        }

        private Expression[] FlattenArguments(Expression[] args)
        {
            var arguments = new List<Expression>();

            foreach (var arg in args)
            {
                if (arg.Type == TypeClass.Normal)
                {
                    Normal norm = arg as Normal;
                    if (norm.Head.Equals(Head))
                    {
                        arguments.AddRange(norm.Arguments);
                    }
                    else
                    {
                        arguments.Add(norm);
                    }
                }
                else
                {
                    arguments.Add(arg);
                }
            }
            return arguments.ToArray();
        }

        public override string Head
        {
            get
            {
                return FunctionHead;
            }
        }
        public override TypeClass Type
        {
            get
            {
                return TypeClass.Normal;
            }
        }
        public virtual int Length
        {
            get
            {
                return Arguments.Length;
            }
        }
        public virtual Expression this [int indexer]
        {
            get
            {
                return Arguments[indexer];
            }
        }

        public override bool Numeric()
        {
            return IsNumeric;
        }
        public override bool Imprecise()
        {
            return this.Any((x) => x.Imprecise());
        }

        public override Expression Substitute(Environment env)
        {
            var args = Arguments.Select((x) => x.Substitute(env));

            return new Normal(FunctionHead, Kernel, args.ToArray());
        }

        protected virtual int ComputeHash()
        {
            int hash = Head.GetHashCode();
            bool orderless = Attributes.HasFlag(NormalAttributes.Orderless);

            int index = 1;

            foreach (Expression exp in this)
            {
                if (!orderless)
                {
                    hash <<= 1;
                    hash ^= index;
                    index++;
                }
                hash ^= exp.GetHashCode();
            }
            return 0;
        }

        public override int GetHashCode()
        {
            return ComputedHash;
        }
        public override bool Equals(object obj)
        {
            Normal norm = obj as Normal;
            if (norm == null || norm.GetHashCode() != GetHashCode() || norm.Head != Head)
            {
                return false;
            }

            if (norm.Length != Length)
            {
                return false;
            }
            for (int i = 0; i < Length; i++)
            {
                if (!this[i].Equals(norm[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public override string FullForm()
        {
            StringBuilder bldr = new StringBuilder();
            bldr.Append(Head);
            bldr.Append('(');

            if (Arguments.Length > 0)
            {
                bldr.Append(Arguments[0].FullForm());
            }
            for (int i = 1; i < Arguments.Length; i++)
            {
                bldr.Append(", ");
                bldr.Append(Arguments[i].FullForm());
            }
            bldr.Append(")");
            return bldr.ToString();
        }
        public override string PrettyForm()
        {
            StringBuilder bldr = new StringBuilder();
            bldr.Append(Head);
            bldr.Append('(');

            if (Arguments.Length > 0)
            {
                bldr.Append(Arguments[0].FullForm());
            }
            for (int i = 1; i < Arguments.Length; i++)
            {
                bldr.Append(", ");
                bldr.Append(Arguments[i].PrettyForm());
            }
            bldr.Append(")");
            return bldr.ToString();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            for (int i = 0; i < Length; i++)
            {
                yield return this[i];
            }
        }
        // C# has really ugly syntax for this
        public virtual IEnumerator<Expression> GetEnumerator()
        {
            for (int i = 0; i < Length; i++)
            {
                yield return this[i];
            }
        }
    }
}

