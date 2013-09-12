using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Rw.Evaluation;
using Rw.Parsing;

namespace Rw
{
    /// <summary>
    /// Represents an expression of many arguments.
    /// Examples include functions (log(x)), or operators
    /// add(2, 2), or other types of compound expressions.
    /// </summary>
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

            Arguments = Attributes.HasFlag(NormalAttributes.Flat) ?
                FlattenArguments(args) : args;
            IsNumeric = Attributes.HasFlag(NormalAttributes.Numeric) && 
                this.All((x) => x.Numeric());

            ComputedHash = ComputeHash();
            foreach (var arg in Arguments)
            {
                Variables.UnionWith(arg.FreeVariables());
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

        /// <summary>
        /// Gives the number of arguments to this normal.
        /// </summary>
        public virtual int Length
        {
            get
            {
                return Arguments.Length;
            }
        }
        /// <summary>
        /// Gets the i'th argument of this normal expression.
        /// </summary>
        /// <param name='indexer'>
        /// Index of the argument to access.
        /// </param>
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
        public override Expression AsImprecise()
        {
            return Create(this.Select((x) => x.AsImprecise()).ToArray());
        }

        public override bool Negative()
        {
            if (Head == "multiply")
            {
                return this.Where((x) => x.Negative()).Count() % 2 == 1;
            }
            return base.Negative();
        }
        public override Expression AsNonnegative()
        {
            if (Head == "multiply")
            {
                var abs = this.Select((x) => x.AsNonnegative());
                if (abs.Count() == 1)
                {
                    return abs.First();
                }
                abs = abs.Where((x) => !x.Equals(new Integer(1, Kernel)));
                if (abs.Count() == 0)
                {
                    return new Integer(1, Kernel);
                }
                if (abs.Count() == 1)
                {
                    return abs.First();
                }
                return Create(abs.ToArray());
            }
            return base.AsNonnegative();
        }

        /// <summary>
        /// Creates a new normal of the same type with the specific
        /// arguments.
        /// </summary>
        /// <param name='args'>
        /// Arguments to the new normal expression.
        /// </param>
        public virtual Normal Create(params Expression[] args)
        {
            return new Normal(FunctionHead, Kernel, args);
        }

        public override Expression Substitute(Environment env)
        {
            var args = Arguments.Select((x) => x.Substitute(env));

            return new Normal(FunctionHead, Kernel, args.ToArray());
        }

        public override bool TryEvaluate(Lookup rules, out Expression evaluated)
        {
            if (TryEvaluateInner(rules, out evaluated))
            {
                return true;
            }
            return TryEvaluateOuter(rules, out evaluated);
        }
        private bool TryEvaluateOuter(Lookup rules, out Expression evaluated)
        {
            foreach (var rule in rules.ApplicableRules(this))
            {
                if (rule.Apply(this, out evaluated))
                {
                    return true;
                }
            }
            evaluated = null;
            return false;
        }
        private bool TryEvaluateInner(Lookup rules, out Expression evaluated)
        {
            var copy = this.ToArray();
            Expression arg;
            for (int i = 0; i < copy.Length; i++)
            {
                if (copy[i].TryEvaluate(rules, out arg))
                {
                    copy[i] = arg;
                    evaluated = Create(copy);
                    return true;
                }
            }
            evaluated = null;
            return false;
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
            return this.SequenceEqual(norm);
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
            if (this.RequiresPrettyPrint())
            {
                return this.PrettyPrint();
            }
            StringBuilder bldr = new StringBuilder();
            bldr.Append(Head);
            bldr.Append('(');

            if (Arguments.Length > 0)
            {
                bldr.Append(Arguments[0].PrettyForm());
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
        /// <summary>
        /// Exposes the normal as an IEnumerable over its arguments.
        /// Makes it easier for pattern matching to iterate and allows
        /// other classes to provide their own functionality for iteration.
        /// </summary>
        /// <returns>
        /// The enumerator.
        /// </returns>
        public virtual IEnumerator<Expression> GetEnumerator()
        {
            for (int i = 0; i < Length; i++)
            {
                yield return this[i];
            }
        }
    }
}

