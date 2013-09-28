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
    }
}

