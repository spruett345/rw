using System;

namespace Rw
{
    public class Decimal : Expression
    {
        public readonly decimal Value;

        private readonly int ComputedHash;

        public Decimal(decimal val, Kernel kernel) : base(kernel)
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
                return TypeClass.Decimal;
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
            return false;
        }
    }
}

