using System;
using System.Linq;
using System.Numerics;
using Rw;
using Rw.Matching;
using Rw.Evaluation;
using Rw.Parsing.Hand;

namespace Rw.Console
{
    using Console = System.Console;
    class MainClass
    {
        public static void Main(string[] args)
        {
            string rules = System.IO.File.ReadAllText("../../../Kernel.rw");

            Kernel kernel = new Kernel();
            kernel.Parse(rules);
            Parser testParser = new Parser("let x = 1, y = 2 in x y z", kernel);
            Console.WriteLine(kernel.Evaluate(testParser.ParseExpression()));
            while (true)
            {
                Console.Write(" > ");
                string input = Console.ReadLine();

                try
                {
                    Parser parser = new Parser(input, kernel);
                    Console.WriteLine(" = " + kernel.Evaluate(parser.ParseExpression()));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(" >>> " + ex.ToString());
                }
                /*try
                {
                    kernel.Parse(Console.ReadLine(), (exp, eval) => { 
                        Console.Write(exp + " -> ");
                        Console.WriteLine(eval); 
                        return true;
                    }
                    );
                } 
                catch (Exception e)
                {
                    Console.WriteLine(" >>> " + e.Message);
                }*/
            }
        }
    }
}
