using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;

namespace Rw
{
    /// <summary>
    /// Aribtrary sized integer expression type.
    /// </summary>
    public class Integer : Number<BigInteger>
    {
        public Integer(BigInteger value, Kernel kernel) :  base(value, kernel)
        {

        }

        public override string Head
        {
            get
            {
                return "int";
            }
        }
        public override Expression Apply(params Expression[] arguments)
        {
            var args = new List<Expression>();
            args.Add(this);
            args.AddRange(arguments);
            return new Normal("multiply", Kernel, args.ToArray());
        }

        public override Decimal DecimalValue()
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
    }
}

