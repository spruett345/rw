using System;
using System.Collections.Generic;

namespace Rw
{
    /// <summary>
    /// A base class for all Expression types.
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

        public virtual Expression Substitute(Environment env)
        {
            return this;
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

