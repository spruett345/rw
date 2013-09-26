using System;

namespace Rw.Parsing.Hand
{
    public class Operator
    {
        public string Value;
        public int Precedence;
        public bool RightAssociative;
        public bool Unary;

        public Operator(string val, int prec, bool right = false, bool unary = false)
        {
            Value = val;
            Precedence = prec;
            RightAssociative = right;
            Unary = unary;
        }
    }
}

