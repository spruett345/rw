using System;
using System.Collections.Generic;
using Rw.Evaluation;

namespace Rw
{
    /// <summary>
    /// A base class for all Expression types.
    /// Defines an interface for methods to be implemented to express
    /// common functionality, and provides several default implementations.
    /// </summary>
    public abstract class Expression
    {
        protected HashSet<Symbol> Variables;

        public abstract string Head { get; }

        public abstract TypeClass Type { get; }

        public readonly Kernel Kernel;

        public Expression(Kernel kernel)
        {
            Kernel = kernel;
            Variables = new HashSet<Symbol>();
        }

        /// <summary>
        /// Gives whether this expression is decidedly negative or
        /// less than zero. True if this < 0, false if not or 
        /// undecidable.
        /// </summary>
        public virtual bool Negative()
        {
            return false;
        }

        public virtual Expression AsNonnegative()
        {
            return this;
        }

        public virtual bool Numeric()
        {
            return false;
        }

        public virtual bool Imprecise()
        {
            return false;
        }
        public virtual Expression AsImprecise()
        {
            return this;
        }

        public virtual Expression Substitute(Environment env)
        {
            return this;
        }
        public virtual Expression Evaluate(Lookup rules = null)
        {
            if (rules == null)
            {
                rules = Kernel.DefaultRules();
            }
            Expression evaluated = this;
            if (evaluated.Imprecise())
            {
                evaluated = evaluated.AsImprecise();
            }
            Expression prev = evaluated;
            while (prev.TryEvaluate(rules, out evaluated))
            {
                prev = evaluated;
            }
            return prev;
        }
        public virtual bool TryEvaluate(Lookup rules, out Expression evaluated)
        {
            evaluated = null;
            return false;
        }

        public virtual ISet<Symbol> FreeVariables()
        {
            return Variables;
        }

        public abstract string FullForm();

        public virtual string PrettyForm()
        {
            return FullForm();
        }
        
        public override string ToString()
        {
            return PrettyForm();
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            return false;
        }
    }
}

