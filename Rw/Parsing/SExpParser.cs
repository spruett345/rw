using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Numerics;
using System.Collections.Generic;

namespace Rw.Parsing
{
    public class SExpParser
    {
        private readonly string Input;
        private int Index;

        private readonly Kernel Kernel;

        private static char[] Whitespace = new char[] { ' ', '\t', '\r', '\n', ',' };
        private static char[] Special = new char[] { '(', ')' };

        public SExpParser(string input, Kernel kernel)
        {
            Input = input;
            Index = 0;

            Kernel = kernel;
        }

        public Expression Parse()
        {
            string token = ReadToken();

            return Parse(token);
        }

        private Expression Parse(string token)
        {
            if (token != "(")
            {
                if (Regex.IsMatch(token, @"^(\+|-)?\d+$"))
                {
                    return new Integer(BigInteger.Parse(token), Kernel);
                }
                if (Regex.IsMatch(token, @"^[+-]?(\d+(\.\d*)?|\.\d+)([eE][+-]?\d+)?$"))
                {
                    return new Decimal(double.Parse(token), Kernel);
                }
                return new Symbol(token, Kernel);
            } 
            else
            {
                string head = ReadToken();

                string next = ReadToken();

                var args = new List<Expression>();
                while (next != ")")
                {   
                    args.Add(Parse(next));
                    next = ReadToken();
                }
                return new Normal(head, Kernel, args.ToArray());
            }
            throw new Exception("Unexpected token in parsing steam " + token);
        }

        private string ReadToken()
        {
            ignoreWhitespace();

            StringBuilder retn = new StringBuilder();

            char c = Input[Index];

            if (Special.Contains(c))
            {
                Index++;
                return c.ToString();
            }
            while (!Special.Contains(c) && !Whitespace.Contains(c))
            {
                retn.Append(c);

                Index++;
                if (Index >= Input.Length)
                {
                    break;
                }
                c = Input[Index];
            }
            return retn.ToString();
        }

        private void ignoreWhitespace()
        {
            while (Index < Input.Length && Whitespace.Contains(Input[Index]))
            {
                Index++;
            }
        }
    }
}

