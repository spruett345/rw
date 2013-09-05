using System;
using System.Linq;
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
            var q = new int[] { 1, 1, 2}.Except(new int[] { 1 });
            foreach (var a in q)
            {
                Console.WriteLine(a);
            }
            Kernel kernel = new Kernel();
            kernel.NormalAttributes["nm"] = NormalAttributes.Numeric;
            kernel.NormalAttributes["add"] = NormalAttributes.Flat | NormalAttributes.Orderless;
            var pattern = new NormalPattern("add", 
                                            new BoundPattern(new UntypedPattern(), "x"),
                                            new BoundPattern(new TypedPattern("int"), "x"), 
                                            new BoundPattern(new TypedPattern("sym"), "y"), 
                                            new BoundPattern(new UntypedPattern(), "z"));

            while (true)
            {
                string line = Console.ReadLine();
                var parser = new SExpParser(line, kernel);
                var exp = parser.Parse();
                Console.WriteLine(exp);
                MatchEnvironment env = new MatchEnvironment();
                if (pattern.Matches(exp, env))
                {
                    Console.WriteLine("Matches!");
                    Console.WriteLine("x -> " + env["x"]);
                    Console.WriteLine("y -> " + env["y"]);
                    Console.WriteLine("z -> " + env["z"]);
                }
                int time = System.Environment.TickCount;
                for (int i = 0; i < 10000; i++)
                {
                    pattern.Matches(exp, env);
                }
                Console.WriteLine(System.Environment.TickCount - time);
            }
        }
    }
}
