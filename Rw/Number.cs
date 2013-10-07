using System;
using System.Collections.Generic;

namespace Rw
{
    public abstract class Number : Expression
    {
        public Number(Kernel kernel) : base(kernel)
        {

        }
        public override TypeClass Type
        {
            get
            {
                return TypeClass.Number;
            }
        }

        public abstract int Sign { get; }
        public abstract Decimal AsDecimal();


        public override bool Negative()
        {
            return Sign < 0;
        }
        public override Expression AsImprecise()
        {
            return AsDecimal();
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

        public virtual int Compare(Number other)
        {
            return AsDecimal().Value.CompareTo(other.AsDecimal().Value);
        }

        public virtual Number Multiply(Number other)
        {
            Integer integer = other as Integer;
            if (integer != null)
            {
                return Multiply(integer);
            }
            Rational rational = other as Rational;
            if (rational != null)
            {
                return Multiply(rational);
            }
            Decimal dec = other.AsDecimal();
            return Multiply(dec);
        }
        public virtual Number Add(Number other)
        {
            Integer integer = other as Integer;
            if (integer != null)
            {
                return Add(integer);
            }
            Rational rational = other as Rational;
            if (rational != null)
            {
                return Add(rational);
            }
            Decimal dec = other.AsDecimal();
            return Add(dec);
        }
        public abstract Number Multiply(Integer integer);
        public abstract Number Multiply(Rational rational);
        public virtual Number Multiply(Decimal dec)
        {
            return AsDecimal().Multiply(dec);
        }
        public abstract Number Add(Integer integer);
        public abstract Number Add(Rational rational);
        public virtual Number Add(Decimal dec)
        {
            return AsDecimal().Add(dec);
        }
    }
}

