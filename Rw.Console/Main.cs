using System;
using Rw;
using Rw.Parsing;
using Rw.Matching;

namespace Rw.Console
{
    using Console = System.Console;
    class MainClass
    {
        public static void Main(string[] args)
        {
            Kernel kernel = new Kernel();
            kernel.NormalAttributes["nm"] = NormalAttributes.Numeric;
            var pattern = new NormalPattern("add", new BoundPattern(new UntypedPattern(), "x"), new BoundPattern(new TypedPattern("num"), "x"));
            while (true)
            {
                string line = Console.ReadLine();
                var parser = new SExpParser(line, kernel);
                var exp = parser.Parse();
                Console.WriteLine(exp);
                Console.WriteLine("Matches: " + pattern.Matches(exp, new MatchEnvironment()));
            }
        }
    }
}
