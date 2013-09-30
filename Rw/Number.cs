using System;

namespace Rw
{
    public abstract class Number<T> : NativeExpression<T> 
    {
        public Number(T value, Kernel kernel) : base(value, kernel)
        {

        }

        public abstract Decimal DecimalValue();
        public virtual int Sign
        {
            get
            {
                return 0;
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
            return Sign < 0;
        }
        public override Expression AsImprecise()
        {
            return DecimalValue();
        }

        private static void Multiply <T1, T2>(Number<T1> left, Number<T2> right)
        {
            if (T1 is Double || T2 is Double)
            {
                return new Decimal(left.DecimalValue().Value * right.DecimalValue().Value, left.Kernel);
            }
        }
    }
}

