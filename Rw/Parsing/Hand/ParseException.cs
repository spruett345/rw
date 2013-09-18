using System;

namespace Rw
{
    public class ParseException : Exception
    {
        public ParseException(string message, int line) 
            : base("Error in parsing on line " + line + ": " + message)
        {
        }
    }
}

