using System;
using System.Linq;
using System.Numerics;
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
            
            Kernel kernel = new Kernel();

            kernel.NormalAttributes["nm"] = NormalAttributes.Numeric;
            kernel.NormalAttributes["add"] = NormalAttributes.Flat | NormalAttributes.Orderless;
            kernel.NormalAttributes["multiply"] = NormalAttributes.Flat | NormalAttributes.Orderless;
            var pattern = new NormalPattern("add",
                                            new BoundPattern(new TypedPattern("int"), "x"),
                                            new BoundPattern(new TypedPattern("int"), "y")
                                            );
            var mulpattern = new NormalPattern("multiply",
                                               new BoundPattern(new TypedPattern("int"), "x"),
                                               new BoundPattern(new TypedPattern("int"), "y")
                                                   );
            var powpattern = new NormalPattern("pow",
                                               new BoundPattern(new TypedPattern("int"), "x"),
                                               new BoundPattern(new TypedPattern("int"), "y")
                                                   );
            var rule = new DefinedRule(pattern, (env) => new Integer((env["x"] as Integer).Value + (env["y"] as Integer).Value, kernel));
            var rule2 = new DefinedRule(mulpattern, (env) => new Integer((env["x"] as Integer).Value * (env["y"] as Integer).Value, kernel));
            /*var rule3 = new DefinedRule(powpattern, (env) => new Integer(
                BigInteger.Pow(
                    (env["x"] as Integer).Value, 
                    (int)(env["y"] as Integer).Value), 
                kernel));*/
            kernel.AddRule("multiply", rule2);
            kernel.AddRule("add", rule);
            //kernel.AddRule("pow", rule3);

            var lahead = new NormalPattern("d", new ConstantPattern("x"), new BoundPattern(new UntypedPattern(), "x"));
            var lrule = new DefinedRule(lahead, (env) => new Integer(0, kernel));
            kernel.AddRule("d", lrule);
            while (true)
            {
                Console.Write(" > ");
                Parser parser = new Parser(Console.ReadLine(), kernel);
                parser.ParseProgram();
                /*string line = Console.ReadLine();
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
                Console.WriteLine(System.Environment.TickCount - time);*/
            }
        }
    }
}
