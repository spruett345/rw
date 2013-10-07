using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;

namespace Rw
{
    /// <summary>
    /// Aribtrary sized integer expression type.
    /// </summary>
    public class Integer : Number
    {
        public readonly BigInteger Value;
        private readonly int ComputedHash;

        public Integer(BigInteger value, Kernel kernel) :  base(kernel)
        {
            Value = value;

            ComputedHash = ComputeHash();
        }

        private int ComputeHash()
        {
            return "decimal".GetHashCode() ^ Value.GetHashCode();
        }

        public override int Sign
        {
            get
            {
                return Value.Sign;
            }
        }
        public override string Head
        {
            get
            {
                return "int";
            }
        }
        public override Decimal AsDecimal()
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
        public override int GetHashCode()
        {
            return ComputedHash;
        }
        public override bool Equals(object obj)
        {
            Integer integer = obj as Integer;
            if (integer != null)
            {
                return integer.Value == Value;
            }
            return false;
        }
        public override Number Add(Integer integer)
        {
            return new Integer(Value + integer.Value, Kernel);
        } 
        public override Number Add(Rational rational)
        {
            return AsRational().Add(rational);
        }
        public override Number Multiply(Integer integer)
        {
            return new Integer(Value * integer.Value, Kernel);
        }
        public override Number Multiply(Rational rational)
        {
            return AsRational().Multiply(rational);
        }

        public override string FullForm()
        {
            return Value.ToString();
        }
        public Rational AsRational()
        {
            return new Rational(Value, 1, Kernel);
        }
    }
}

