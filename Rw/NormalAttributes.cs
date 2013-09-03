using System;

namespace Rw
{
    [Flags]
    public enum NormalAttributes
    {
        None = 0,
        Numeric = 1 << 0,
        Flat = 1 << 1,
        Orderless = 1 << 2,
        Operator = 1 << 3
    }
}