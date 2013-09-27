using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;

namespace Rw
{
    /// <summary>
    /// Aribtrary sized integer expression type.
    /// </summary>
    public class Integer : Expression
    {
        /// <summary>
        /// The value of this integer expression as a
        /// BigInteger.
        /// </summary>
        public readonly BigInteger Value;

        private readonly int ComputedHash;

        public Integer(BigInteger val, Kernel kernel) : base(kernel)
        {
            Value = val;
            ComputedHash = ComputeHash();
        }

        public override string Head
        {
            get
            {
                return "int";
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
            return new Integer(Value < 0 ? Value * -1 : Value, Kernel);
        }

        public override bool Numeric()
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

        public override string FullForm()
        {
            return Value.ToString();
        }

        private int ComputeHash()
        {
            return "int".GetHashCode() ^ Value.GetHashCode();
        }

        public override int GetHashCode()
        {
            return ComputedHash;
        }
        public override bool Equals(object obj)
        {
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

