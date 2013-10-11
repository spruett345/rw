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

        protected int ComputedHash;
        private readonly bool IsNumeric;

        public readonly NormalAttributes Attributes;

        public Normal(string head, Kernel kernel, params Expression[] args) : base(kernel)
        {
            Attributes = Kernel.GetNormalAttributes(head);

            FunctionHead = head;

            Arguments = Attributes.HasFlag(NormalAttributes.Flat) ?
                FlattenArguments(args) : args;
            if (Attributes.HasFlag(NormalAttributes.Orderless))
            {
                Array.Sort(Arguments, (x, y) => ((x.Type - y.Type) << 28 | (x.GetHashCode() - y.GetHashCode()) >> 4));
            }
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

        private bool FoldConstants(out Expression reduced)
        {
            if (Head == "add")
            {
                return ReduceNumbers((x, y) => x.Add(y), out reduced);
            }
            if (Head == "multiply")
            {
                return ReduceNumbers((x, y) => x.Multiply(y), out reduced);
            }
            reduced = null;
            return false;
        }
        private bool ReduceNumbers(Func<Number, Number, Number> fold, out Expression reduced)
        {
            IEnumerable<Expression> others;
            var numbers = SelectNumbers(out others);
            if (numbers.Count() >= 2)
            {
                var num = numbers.First();
                foreach (var n in numbers.Skip(1))
                {
                    num = fold(num, n);
                }
                if (others.Count() == 0)
                {
                    reduced = num;
                    return true;
                }
                else
                {
                    var prm = others.ToList();
                    prm.Add(num);
                    reduced = Create(prm.ToArray());
                    return true;
                }
            }
            else
            {
                reduced = null;
                return false;
            }
        }
        private IEnumerable<Number> SelectNumbers(out IEnumerable<Expression> other)
        {
            var numbers = new List<Number>();
            var others = new List<Expression>();

            foreach (var expr in this)
            {
                var num = expr as Number;
                if (num != null)
                {
                    numbers.Add(num);
                }
                else
                {
                    others.Add(expr);
                }
            }
            other = others;
            return numbers;
        
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


        public override Expression Apply(params Expression[] arguments)
        {
            if (Attributes.HasFlag(NormalAttributes.Operator))
            {
                throw new Exception("cannot apply an operator expression like a function. use * for multiply between parentheses");
            }
            var args = new List<Expression>();
            args.AddRange(Arguments);
            args.AddRange(arguments);

            return Create(args.ToArray());
        }

        public override Expression Substitute(Environment env)
        {
            var args = this.Select((x) => x.Substitute(env));

            if (env.ContainsKey(FunctionHead))
            {
                var head = env[FunctionHead];
                return head.Apply(args.ToArray());
            }
            return Create(args.ToArray());
        }

        public override bool TryEvaluate(Lookup rules, out Expression evaluated)
        {
            if (FoldConstants(out evaluated))
            {
                return true;
            }
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
                if (exp == null)
                {
                    continue;
                }

                if (!orderless)
                {
                    hash <<= 1;
                    hash ^= index;
                    index++;
                }
                hash ^= exp.GetHashCode();
            }
            return hash;
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

            if (this.Count() > 0)
            {
                bldr.Append(this.First().FullForm());
            }
            foreach (var expr in this.Skip(1))
            {
                bldr.Append(", ");
                bldr.Append(expr.FullForm());
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

            if (this.Count() > 0)
            {
                bldr.Append(this.First().PrettyForm());
            }
            foreach (var expr in this.Skip(1))
            {
                bldr.Append(", ");
                bldr.Append(expr.PrettyForm());
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

