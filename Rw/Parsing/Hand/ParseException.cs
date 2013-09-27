using System;

namespace Rw
{
    public class ParseException : Exception
    {
        public readonly bool LineNumber = false;
        public ParseException(string message, int line) 
            : base("Error in parsing on line " + line + ": " + message)
        {
            LineNumber = true;
        }
         public ParseException(string message) 
            : base(message)
        {
        }
    }
}

