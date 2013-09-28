using System;
using System.Linq;
using System.Collections.Generic;

namespace Rw
{
    /// <summary>
    /// Imprecise floating point decimal type expression.
    /// Internally uses 128-bit decimals for maximum precision.
    /// </summary>
    public class Decimal : Expression
    {
        /// <summary>
        /// The value of this Decimal expression as a system
        /// 128-bit decimal.
        /// </summary>
        public readonly double Value;

        private readonly int ComputedHash;

        public Decimal(double val, Kernel kernel) : base(kernel)
        {
            Value = val;
            ComputedHash = ComputeHash();
        }

        public override string Head
        {
            get
            {
                return "decimal";
            }
        }
        public override TypeClass Type
        {
            get
            {
                return TypeClass.Number;
            }
        }

        public override bool Negative()
        {
            return Value < 0;
        }

        public override Expression AsNonnegative()
        {
            return new Decimal(Value < 0 ? Value * -1 : Value, Kernel);
        }
        public override string FullForm()
        {
            return Value.ToString();
        }

        public override bool Numeric()
        {
            return true;
        }
        public override bool Imprecise()
        {
            return true;
        }

        public override Expression Apply(params Expression[] arguments)
        {
            var args = new List<Expression>();
            args.Add(this);
            args.AddRange(arguments);
            return new Normal("multiply", Kernel, args.ToArray());
        }

        private int ComputeHash()
        {
            return "decimal".GetHashCode() ^ Value.GetHashCode();
        }

        public override int GetHashCode()
        {
            return ComputedHash;
        }
        public override bool Equals(object obj)
        {
            Decimal dec = obj as Decimal;
            if (dec != null)
            {
                return dec.GetHashCode() == GetHashCode()
                    && dec.Value == Value;
            }
            Integer i = obj as Integer;
            if (i != null)
            {
                return i.Equals(this);
            }
            return false;
        }
    }
}

