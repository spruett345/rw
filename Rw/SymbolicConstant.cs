using System;

namespace Rw
{
    /// <summary>
    /// Constants which should be left in symbolic form unless
    /// asked for numerically. Eg: pi/e.
    /// </summary>
    public class SymbolicConstant : Expression
    {
        /// <summary>
        /// The value of the constant in double form.
        /// </summary>
        public readonly double Value;

        /// <summary>
        /// The name of the constant, eg: pi.
        /// </summary>
        public readonly string Name;

        private readonly int ComputedHash;

        public SymbolicConstant(string name, double val, Kernel kernel) : base(kernel)
        {
            Name = name;
            Value = val;
            ComputedHash = ComputeHash();
        }

        public override string Head
        {
            get
            {
                return "symconst";
            }
        }
        public override TypeClass Type
        {
            get
            {
                return TypeClass.Integer;
            }
        }

        public override bool Negative()
        {
            return Value < 0;
        }
        public override Expression AsNonnegative()
        {
            return this;
        }

        public override bool Numeric()
        {
            return true;
        }

        public override Expression AsImprecise()
        {
            try
            {
                return new Decimal((double)Value, Kernel);
            } 
            catch (OverflowException)
            {
                return new Decimal(double.MaxValue, Kernel);
            }
        }

        public override Expression Apply(params Expression[] arguments)
        {
            throw new System.NotImplementedException();
        }

        public override string FullForm()
        {
            return Name.ToString();
        }

        private int ComputeHash()
        {
            return "symconst".GetHashCode() ^ Value.GetHashCode() ^ Name.GetHashCode();
        }

        public override int GetHashCode()
        {
            return ComputedHash;
        }
        public override bool Equals(object obj)
        {
            SymbolicConstant cons = obj as SymbolicConstant;
            if (cons != null)
            {
                return cons.GetHashCode() == GetHashCode() &&
                    cons.Name == Name && cons.Value == Value;
            }
            Integer i = obj as Integer;
            if (i != null)
            {
                return i.GetHashCode() == GetHashCode() &&
                    i.Value.Equals(Value);
            }
            Decimal d = obj as Decimal;
            if (d != null)
            {
                try
                {
                    return d.Value == (double)Value;
                }
                catch (OverflowException)
                {
                    return false;
                }
            }
            return false;
        }
    }
}

