using System;
using System.Linq;
using Rw;
using Rw.Parsing;
using Rw.Matching;
using Rw.Evaluation;
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
                                            new BoundPattern(new TypedPattern("int"), "x"),
                                            new BoundPattern(new TypedPattern("int"), "y")
                                            );
            var rule = new DefinedRule(pattern, (env) => new Integer((env["x"] as Integer).Value + (env["y"] as Integer).Value, kernel));
            kernel.AddRule("add", rule);
            while (true)
            {
                string line = Console.ReadLine();
                var parser = new SExpParser(line, kernel);
                var exp = parser.Parse();
                Console.WriteLine(exp);
                MatchEnvironment env = new MatchEnvironment();
                Expression applied;
                Console.WriteLine("-> " + exp.Evaluate());
                
                int time = System.Environment.TickCount;
                for (int i = 0; i < 10000; i++)
                {
                    exp.Evaluate();
                }
                Console.WriteLine(System.Environment.TickCount - time);
            }
        }
    }
}
