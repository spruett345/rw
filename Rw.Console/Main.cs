using System;
using System.Linq;
using System.Numerics;
using Rw;
using Rw.Parsing;
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

            while (true)
            {
                Console.Write(" > ");
                Tokenizer tokens = new Tokenizer(Console.ReadLine() + "\n" + Console.ReadLine());
                foreach (var token in tokens.Tokens())
                {
                    Console.WriteLine(token.Value + " [" + token.Type + "] on line " + tokens.GetLineNumber());
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
