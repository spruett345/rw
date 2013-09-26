using System;

namespace Rw
{
    /// <summary>
    /// Represents a variable, or symbol in an expression.
    /// </summary>
    public class Symbol : Expression
    {
        /// <summary>
        /// The name of this symbol, e.g. 'x' or 'y'.
        /// </summary>
        public readonly string Name;
        private readonly int ComputedHash;
        
        public Symbol(string name, Kernel kernel) : base(kernel)
        {
            Name = name;

            ComputedHash = ComputeHash();
            Variables.Add(this);
        }

        public override string Head
        {
            get
            {
                return "sym";
            }
        }

        public override TypeClass Type
        {
            get
            {
                return TypeClass.Symbol;
            }
        }

        public override Expression Substitute(Environment env)
        {
            if (env.ContainsKey(Name))
            {
                return env[Name];
            }
            return this;
        }

        public override Expression Invoke(params Expression[] arguments)
        {
            return new Normal(Name, Kernel, arguments);
        }

        public override string FullForm()
        {
            return Name;
        }

        private int ComputeHash()
        {
            return "sym".GetHashCode() ^ Name.GetHashCode();
        }

        public override int GetHashCode()
        {
            return ComputedHash;
        }

        public override bool Equals(object obj)
        {
            Symbol sym = obj as Symbol;
            if (sym != null)
            {
                return sym.GetHashCode() == GetHashCode() &&
                    sym.Name.Equals(Name);
            }
            return false;
        }
    }
}

