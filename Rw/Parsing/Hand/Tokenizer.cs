using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Rw.Parsing.Hand
{
    /// <summary>
    /// A tokenizer which parses string input and returns
    /// a string of tokens to be turned into an AST.
    /// </summary>
    public class Tokenizer
    {
        private static string[] Symbols = new string[] {
            "+", "-", "/", "*", "^", ">", "<", ">=", "<=",
            "=", "!=", "and", "or", ":=", ";", "(", ")"
        };

        private static string[] Keywords = new string[] {
            "let", "def", "when", "in"
        };

        private int Index;
        private string Input;

        public Tokenizer(string input)
        {
            Input = Sanitize(input);
            Index = 0;
        }

        public IEnumerable<Token> Tokens()
        {
            Token cur;
            while (TryReadToken(out cur))
            {
                yield return cur;
            }
            yield return new Token("", TokenType.End);
        }

        private bool TryReadToken(out Token token)
        {
            IterateWhitespace();
            if (Index >= Input.Length)
            {
                token = null;
                return false;
            }

            var bldr = new StringBuilder();
            char current = Input[Index];
            Index++;

            bldr.Append(current);

            if (current == '\n')
            {
                token = new Token("\n", TokenType.NewLine);
                return true;
            }
            if (ValidNumberStart(current))
            {
                var state = Index;
                if (ParseNumber(bldr))
                {
                    string val = bldr.ToString();
                    TokenType numType = val.Contains(".") || val.Contains("e") 
                        || val.Contains("E") ? TokenType.Decimal : TokenType.Integer;

                    token = new Token(bldr.ToString(), numType);
                    return true;
                }
                Index = state;
            }
            if (ValidTokenStart(current))
            {
                string id = ParseIdentifier(bldr);
                token = new Token(id, Keywords.Contains(id) ? 
                                  TokenType.Keyword : TokenType.Identifier
                );
                return true;
            }
            else if (SymbolStart(current.ToString()))
            {
                string sym = ParseSymbol(bldr.ToString());
                token = new Token(sym, TokenType.Symbol);
                return true;
            }
            
            token = new Token(bldr.ToString(), TokenType.None);
            return false;
        }

        private string ParseIdentifier(StringBuilder start)
        {
            while (Index < Input.Length && ValidTokenChar(Input[Index]))
            {
                start.Append(Input[Index]);
                Index++;
            }
            return start.ToString();
        }

        private bool ValidNumberStart(char c)
        {
            return c == '.' || c == '-' || char.IsDigit(c);
        }
        private bool ValidNumberChar(char c)
        {
            return c == '.' || c == 'e' || c == 'E' || char.IsDigit(c);
        }
        private bool ParseNumber(StringBuilder start)
        {
            bool point = start[0] == '.';
            bool e = false;

            while (Index < Input.Length && ValidNumberChar(Input[Index]))
            {
                char c = Input[Index];
                if (c == '.' && point)
                {
                    break;
                }
                else if (c == '.')
                {
                    point = true;
                }

                if (c == 'e' || c == 'E' && e)
                {
                    break;
                }
                else if (c == 'e' || c == 'E')
                {
                    e = true;
                }

                start.Append(c);

                Index++;
            }
            decimal d;
            return decimal.TryParse(start.ToString(), out d);
        }

        private bool ValidTokenStart(char c)
        {
            return char.IsLetter(c) || c == '_';
        }
        private bool ValidTokenChar(char c)
        {
            return ValidTokenStart(c) || char.IsDigit(c);
        }

        private bool SymbolStart(string c)
        {
            return Symbols.Any(x => x.StartsWith(c.ToString()));
        }
        private string ParseSymbol(string cur)
        {
            var symbols = Symbols.Where(x => x.StartsWith(cur));
            int count = symbols.Count();
            if (count == 0)
            {
                throw new ParseException("tried to match symbol " + cur +
                    " but could not find a match", GetLineNumber());
            }
            if (count == 1)
            {
                string match = symbols.First();

                if (!TryMatchSymbol(cur, match))
                {
                    throw new ParseException("expected symbol " + match +
                        " but got " + cur, GetLineNumber());
                }
                return match;
            }

            if (Index >= Input.Length)
            {
                return cur;
            }
            Index++;
            string next = cur + Input[Index];

            if (!SymbolStart(next))
            {
                return cur;
            }
            return ParseSymbol(next);
        }

        private bool TryMatchSymbol(string start, string match)
        {
            while (start.Length != match.Length)
            {
                if (Index >= Input.Length)
                {
                    throw new ParseException("end of input when trying to parse " 
                        + start + " against symbol " + match, GetLineNumber());
                }
                start += Input[Index];
                Index++;
            }
            return start == match;
        }
        private bool Whitespace()
        {
            return Index < Input.Length && Input[Index] == ' ';
        }
        private void IterateWhitespace()
        {
            while (Whitespace())
            {
                Index++;
            }
        }
        private string Sanitize(string input)
        {
            return input.Replace("\r\n", "\n").Replace("\t", "    ");
        }
        
        public int GetLineNumber()
        {
            int line = 1;
            for (int i = 0; i < Index; i++)
            {
                if (Input[i] == '\n')
                {
                    line++;
                }
            }
            return line;
        }
    }
}

