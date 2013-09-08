using System;

namespace Rw
{
    public class Parser
    {
        private static string[] Keywords = {
            "if", "then", "else", "let", "def", "where"
        };
        private static string[] Symbols = {
            "(", ")", ",", "+", "-", "*", "/", "^",
            "="
        };

        private string[] Lines;

        private int LineIndex;
        private int CharIndex;

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
        private char Current()
        {
            return Lines[LineIndex][CharIndex];
        }
        private bool HasNext()
        {
            return LineIndex < Lines.Length;
        }
        private bool TryReadToken(out string token)
        {
            token = null;
            ReadWhitespace();
            if (!HasNext())
            {
                return false;
            }
            if (CharIndex >= Lines[LineIndex].Length)
            {
                CharIndex = 0;
                LineIndex++;
                return TryReadToken(out token);
            }

            token = "";
            char c = Lines[LineIndex][CharIndex];

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

        private string Sanitize(string input)
        {
            return input.Replace("\r\n", "\n").Replace("\t", "    ");
        }
    }
}

