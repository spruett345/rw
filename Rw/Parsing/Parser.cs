using System;
using System.Linq;
using System.Collections.Generic;

namespace Rw
{
    public class Parser
    {
        private static string[] Keywords = {
            "if", "then", "else", "let", "def", "where", "in"
        };
        private static string[] Symbols = {
            "(", ")", ",", "+", "-", "*", "/", "^",
            "=", ":", ">", "<", ">=", "<=", "[", "]",
            "'", "\"", "."
        };

        private static Dictionary<string, int> OperatorPrecedence = new Dictionary<string, int>();

        private string[] Lines;

        private int LineIndex;
        private int CharIndex;

        static Parser()
        {
            OperatorPrecedence["+"] = 1;
            OperatorPrecedence["*"] = 2;
            OperatorPrecedence["/"] = 2;
            OperatorPrecedence["^"] = 3;
            OperatorPrecedence["-"] = 4;
        }
        public Parser(string input)
        {
            input = Sanitize(input);
            Lines = input.Split('\n');
        }

        private bool Step()
        {
            CharIndex++;
            if (CharIndex == Lines[LineIndex].Length)
            {
                CharIndex = 0;
                LineIndex++;
                if (LineIndex == Lines.Length)
                {
                    return false;
                }
            }
            return true;
        }
        private bool StepToEndLine()
        {
            CharIndex++;
            if (CharIndex == Lines[LineIndex].Length)
            {
                CharIndex = 0;
                LineIndex++;
                return false;
            }
            return true;
        }

        private char Current()
        {
            return Lines[LineIndex][CharIndex];
        }
        private bool HasNext()
        {
            return LineIndex < Lines.Length;
        }
        private bool TryReadToken(out string token, out Token type)
        {
            token = null;
            type = Token.None;

            ReadWhitespace();
            if (!HasNext())
            {
                return false;
            }
            
            token = "";
            char c = Current();
            token += c;


            if (ParseNumber(c, out token))
            {
                type = Token.Number;
                return true;
            }
            var query = Symbols.Where((x) => x.StartsWith(c.ToString()));

            if (query.Count() > 0)
            {
                token = ParseSymbol(token);
                type = Token.Symbol;
                return true;
            }
            if (IsAlpha(c))
            {
                while (StepToEndLine())
                {
                    if (!IsValidToken(Current()))
                    {
                        break;
                    }
                    token += Current();
                }
                type = Token.Word;
                return true;
            }

            return false;
        }
        private string ParseSymbol(string start)
        {
            var query = Symbols.Where((x) => x.StartsWith(start));
            if (query.Count() == 0)
            {
                throw new Exception("Symbol " + start + " does not match parser symbols");
            }
            if (query.Count() == 1)
            {
                string token = query.First();
                while (start.Length < token.Length)
                {
                    if (!StepToEndLine())
                    {
                        throw new Exception("End of line before symbol " + token + " finished parsing");
                    }
                    start += Current();
                }
                if (start != token)
                {
                    throw new Exception("Undefined symbol " + start + " in stream");
                }
                return start;
            }
            if (!StepToEndLine())
            {
                throw new Exception("End of line before symbol " + start + " finished parsing");
            }
            char c = Current();
            if (IsWhitespace(c))
            {
                throw new Exception("Undefined symbol " + start + " ending with whitespace");
            }
            return ParseSymbol(start + c);
        }

        private bool ParseNumber(char c, out string num)
        {
            string n = "";
            bool point = false;
            n += c;
            while (StepToEndLine())
            {
                if (!IsNumber(Current()))
                {
                    break;
                }
                if (Current() == '.' && point)
                {
                    break;
                }
                if (Current() == '.')
                {
                    point = true;
                }
                n += Current();
            }
            num = n;
            if (n == ".")
            {
                return false;
            }
            return true;
        }

        private void ReadWhitespace()
        {
            do
            {
                char c = Lines[LineIndex][CharIndex];
                if (!IsWhitespace(c))
                {
                    break;
                }
            }
            while (Step());
        }
        private bool IsWhitespace(char c)
        {
            return c == ' ';
        }
        private bool IsAlpha(char c)
        {
            return char.IsLetter(c) || c == '_';
        }
        private bool IsValidToken(char c)
        {
            return char.IsLetterOrDigit(c) || c == '_' || c == '?';
        }
        private bool IsNumber(char c)
        {
            return char.IsDigit(c) || c == '.';
        }

        private string Sanitize(string input)
        {
            return input.Replace("\r\n", "\n").Replace("\t", "    ");
        }
    }
}

