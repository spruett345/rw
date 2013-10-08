using System;
using System.Numerics;
using System.Linq;
using System.Collections.Generic;
using Rw.Mathematics;
using Rw.Evaluation;

namespace Rw
{
    public class Rational : Number
    {
        public readonly BigInteger Numerator;
        public readonly BigInteger Denominator;

        private int ComputedHash;

        public Rational(BigInteger num, BigInteger den, Kernel kernel) : base(kernel)
        {
            Numerator = num;
            Denominator = den;
        }

        private bool Simplify(out Expression value)
        {
            int sign = Numerator.Sign * Denominator.Sign;

            var gcd = IntegerMath.GreatestCommonDivisor(Numerator * Numerator.Sign, 
                                                        Denominator * Denominator.Sign);


            var den = Denominator * Denominator.Sign / gcd;
            var num = Numerator * Numerator.Sign / gcd;
            if (den == 1)
            {
                value = new Integer(num, Kernel);
                return true;
            }

            if (gcd == 1)
            {
                value = null;
                return false;
            }
            value = new Rational(num * sign, den, Kernel);
            return true;
        }

        public override int Sign
        {
            get
            {
                return Numerator.Sign * Denominator.Sign;
            }
        }
        public override string Head
        {
            get
            {
                return "rational";
            }
        }

        public override Expression AsNonnegative()
        {
            return new Rational(Numerator * Numerator.Sign, Denominator * Denominator.Sign, Kernel);
        }

        public override bool Numeric()
        {
            return true;
        }
        public override bool TryEvaluate(Lookup rules, out Expression evaluated)
        {
            return Simplify(out evaluated);
        }
        public override Expression Apply(params Expression[] arguments)
        {
            var args = new List<Expression>();
            args.Add(this);
            args.AddRange(arguments);
            return new Normal("multiply", Kernel, args.ToArray());
        }

        public override Decimal AsDecimal()
        {
            try
            {
                double num = (double)Numerator;
                double den = (double)Denominator;
                return new Decimal(num / den, Kernel);
            } 
            catch (OverflowException)
            {
                return new Decimal(double.MaxValue, Kernel);
            }
        }

        public override string FullForm()
        {
            return Numerator.ToString() + "/" + Denominator.ToString();
        }

        private int ComputeHash()
        {
            return "rational".GetHashCode() ^ Numerator.GetHashCode() 
                ^ Denominator.GetHashCode();
        }

        public override int GetHashCode()
        {
            return ComputedHash;
        }
        public override bool Equals(object obj)
        {
            Rational rat = obj as Rational;
            if (rat != null)
            {
                return rat.GetHashCode() == GetHashCode() &&
                    rat.Numerator == Numerator && rat.Denominator == Denominator;
            }
            return false;
        }

        public override Number Add(Integer integer)
        {
            return Add(integer.AsRational());
        }
        public override Number Multiply(Integer integer)
        {
            return Multiply(integer.AsRational());
        }

        public override Number Add(Rational rational)
        {
            BigInteger num = Numerator * rational.Denominator + Denominator * rational.Numerator;
            BigInteger den = rational.Denominator * Denominator;

            return new Rational(num, den, Kernel);
        }
        public override Number Multiply(Rational rational)
        {
            BigInteger num = rational.Numerator * Numerator;
            BigInteger den = rational.Denominator * Denominator;
            return new Rational(num, den, Kernel);
        }
    }
}

