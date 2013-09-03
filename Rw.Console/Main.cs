using System;
using Rw;
using Rw.Parsing;

namespace Rw.Console
{
    using Console = System.Console;
    class MainClass
    {
        public static void Main(string[] args)
        {
            Kernel kernel = new Kernel();
            while (true)
            {
                string line = Console.ReadLine();
                var parser = new SExpParser(line, kernel);

                Console.WriteLine(parser.Parse().FullForm());
            }
        }
    }
}
