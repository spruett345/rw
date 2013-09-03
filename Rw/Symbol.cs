using System;

namespace Rw
{
	public class Symbol : Expression
	{
		public readonly string Name;

        	private readonly int ComputedHash;
		
		public Symbol(string name, Kernel kernel) : base(kernel)
		{
			Name = name;

            		ComputedHash = ComputeHash();
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

