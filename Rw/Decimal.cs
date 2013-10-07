using System;
using System.Linq;
using System.Collections.Generic;

namespace Rw
{
    /// <summary>
    /// Imprecise floating point decimal type expression.
    /// Internally uses 128-bit decimals for maximum precision.
    /// </summary>
    public class Decimal : Number
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
       
        public override Expression AsNonnegative()
        {
            return new Decimal(Value < 0 ? Value * -1 : Value, Kernel);
        }
        public override string FullForm()
        {
            return Value.ToString();
        }

        public override Decimal AsDecimal()
        {
            return this;
        }
        public override bool Imprecise()
        {
            return true;
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

        public override int Sign
        {
            get
            {
                if (Value < 0)
                {
                    return -1;
                }
                else if (Value == 0)
                {
                    return 0;
                }
                return 1;
            }
        }

        public override Number Multiply(Decimal dec)
        {
            return new Decimal(Value * dec.Value, Kernel);
        }
        public override Number Multiply(Integer integer)
        {
            return Multiply(integer.AsDecimal());
        }
        public override Number Multiply(Rational rational)
        {
            return Multiply(rational.AsDecimal());
        }
        public override Number Add(Decimal dec)
        {
            return new Decimal(Value + dec.Value, Kernel);
        }
        public override Number Add(Integer integer)
        {
            return Add(integer.AsDecimal());
        }
        public override Number Add(Rational rational)
        {
            return Add(rational.AsDecimal());
        }
    }
}

