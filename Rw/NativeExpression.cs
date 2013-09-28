using System;

namespace Rw
{
    public abstract class NativeExpression<T> : Expression
    {
        public readonly T Value;

        private readonly int ComputedHash;

        public NativeExpression(T value, Kernel kernel) : base(kernel)
        {
            Value = value;
            ComputedHash = ComputeHash();
        }

        private int ComputeHash()
        {
            return Head.GetHashCode() ^ Value.GetHashCode();
        }

        public override string FullForm()
        {
            return Value.ToString();
        }
        public override int GetHashCode()
        {
            return ComputedHash;
        }
        public override bool Equals(object obj)
        {
            if (obj.GetHashCode() != GetHashCode())
            {
                return false;
            }
            var expr = obj as NativeExpression<T>;
            if (expr == null)
            {
                return false;
            }
            return expr.Value.Equals(Value);
        }
    }
}

