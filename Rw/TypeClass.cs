using System;

namespace Rw
{
    /// <summary>
    /// TypeClass divides expressions into specific classes based on their
    /// type, from primitives such as Integers and Decimals to Normal expressions.
    /// </summary>
	public enum TypeClass
	{
		None = 0,
		Symbol,
		Normal,
		
		Integer,
		Decimal
	}
}

