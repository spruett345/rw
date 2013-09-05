using System;

namespace Rw
{
    public class Boolean : Expression
    {
        public readonly bool Value;

        public Boolean(bool val, Kernel kernel) : base(kernel)
        {
            Value = val;
        }

        public override string Head
        {
            get
            {
                return "bool";
            }
        }
        public override TypeClass Type
        {
            get
            {
                return TypeClass.Integer;
            }
        }

        public override string FullForm()
        {
            return Value.ToString();
        }

        public override int GetHashCode()
        {
            return "bool".GetHashCode() ^ Value.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            Boolean b = obj as Boolean;
            if (b != null)
            {
                return b.Value == Value;
            }
            return false;
        }
    }
}

