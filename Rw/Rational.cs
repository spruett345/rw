using System;
using System.Numerics;
using System.Linq;
using System.Collections.Generic;

namespace Rw
{
    public class Rational : Expression
    {
        public readonly BigInteger Numerator;
        public readonly BigInteger Denominator;

        private int ComputedHash;

        public Rational(BigInteger numerator, BigInteger denominator, Kernel kernel) : base(kernel)
        {
            Numerator = numerator;
            Denominator = denominator;
        }

        public override string Head
        {
            get
            {
                return "rational";
            }
        }
        public override TypeClass Type
        {
            get
            {
                return TypeClass.Integer;
            }
        }

        public override bool Negative()
        {
            return Numerator.Sign * Denominator.Sign < 0;
        }
        public override Expression AsNonnegative()
        {
            return new Rational(Numerator * Numerator.Sign, Denominator * Denominator.Sign, Kernel);
        }

        public override bool Numeric()
        {
            return true;
        }

        public override Expression Apply(params Expression[] arguments)
        {
            var args = new List<Expression>();
            args.Add(this);
            args.AddRange(arguments);
            return new Normal("multiply", Kernel, args.ToArray());
        }

        public override Expression AsImprecise()
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
    }
}
