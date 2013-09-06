using System;

namespace Rw
{
    public class Parser
    {
        private static string[] Keywords = {
            "if", "then", "else", "let"
        };
        private static string[] Symbols = {
            "(", ")", ",", "+", "-", "*", "/", "^",
            "="
        };

        public Parser()
        {
        }

        private string Sanitize(string input)
        {
            return input.Replace("\r\n", "\n").Replace("\t", "    ");
        }
    }
}

