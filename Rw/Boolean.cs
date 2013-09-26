using System;

namespace Rw
{
    /// <summary>
    /// Boolean typed expression, exposes
    /// boolean literals to the type system.
    /// </summary>
    public class Boolean : Expression
    {
        /// <summary>
        /// The value of this boolean expression.
        /// </summary>
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

        public override Expression Invoke(params Expression[] arguments)
        {
            throw new System.NotImplementedException();
        }

        public override string FullForm()
        {
            return Value.ToString().ToLower();
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

