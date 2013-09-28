using System;

namespace Rw.Matching
{
    public class RationalPattern : Pattern
    {
        private readonly string Numerator;
        private readonly string Denominator;

        public RationalPattern(string numerator, string denominator)
        {
            Numerator = numerator;
            Denominator = denominator;
        }

        public override bool Matches(Expression exp, MatchEnvironment env)
        {
            return exp is Rational;
        }

        public override void Bind(Expression exp, MatchEnvironment env)
        {
            Rational rat = exp as Rational;
            env.Bind(Numerator, this, new Integer(rat.Numerator, rat.Kernel));
            env.Bind(Denominator, this, new Integer(rat.Denominator, rat.Kernel));
        }
    }
}

