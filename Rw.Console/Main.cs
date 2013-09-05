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
            kernel.NormalAttributes["mul"] = NormalAttributes.Flat | NormalAttributes.Orderless;
            var pattern = new NormalPattern("add",
                                            new NormalPattern("mul", 
                                                new BoundPattern(new TypedPattern("num"), "k"), 
                                                new BoundPattern(new TypedPattern("sym"), "x")),

                                            new BoundPattern(new TypedPattern("num"), "k")
                                            );

            while (true)
            {
                string line = Console.ReadLine();
                var parser = new SExpParser(line, kernel);
                var exp = parser.Parse();
                Console.WriteLine(exp);
                MatchEnvironment env = new MatchEnvironment();
                Expression matched;
                Expression rest;
                if (pattern.MatchesPartial(exp, env, out matched, out rest))
                {
                    Console.WriteLine("Matches!");
                    Console.WriteLine("Matched -> " + matched);
                    Console.WriteLine("Rest -> " + rest);
                    foreach (var key in env.Keys())
                    {
                        Console.WriteLine(key + " -> " + env[key]);
                    }
                }
                int time = System.Environment.TickCount;
                for (int i = 0; i < 10000; i++)
                {
                    pattern.Matches(exp, new MatchEnvironment());
                }
                Console.WriteLine(System.Environment.TickCount - time);
            }
        }
    }
}
