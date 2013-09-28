using System;
using System.Numerics;

namespace Rw.Mathematics
{
    public static class IntegerMath
    {
        public static BigInteger GreatestCommonDivisor(BigInteger x, BigInteger y)
        {
            return BigInteger.GreatestCommonDivisor(x, y);
        }
        public static int GreatestCommonDivisor(int x, int y)
        {
            if (x == 0 && y == 0)
            {
                return 0;
            }
            if (x == 0)
            {
                return y;
            }
            if (y == 0)
            {
                return x;
            }

            if (x % 2 == 0 && y % 2 == 0)
            {
                return 2 * GreatestCommonDivisor(x / 2, y / 2);
            }
            if (x % 2 == 0)
            {
                return GreatestCommonDivisor(x / 2, y);
            }
            if (y % 2 == 0)
            {
                return GreatestCommonDivisor(x, y / 2);
            }
            if (x >= y)
            {
                return GreatestCommonDivisor((x - y) / 2, y);
            }
            return GreatestCommonDivisor((y - x) / 2, x);
        }
    }
}

