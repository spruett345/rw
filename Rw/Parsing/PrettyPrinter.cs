using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
namespace Rw.Parsing
{
    public static class PrettyPrinter
    {
        private static Dictionary<string, Func<Normal, string>> Printers;
        static PrettyPrinter()
        {
            Printers = new Dictionary<string, Func<Normal, string>>();
            Printers["add"] = PrintSum;
            Printers["multiply"] = PrintProduct;
            Printers["pow"] = PrintPow;
            Printers["and"] = (x) => PrintOperator(x, "and");
            Printers["or"] = (x) => PrintOperator(x, "or");
            Printers["gte"] = (x) => PrintOperator(x, ">=");
            Printers["lte"] = (x) => PrintOperator(x, "<=");
            Printers["equals"] = (x) => PrintOperator(x, "=");
            Printers["lt"] = (x) => PrintOperator(x, "<");
            Printers["gt"] = (x) => PrintOperator(x, ">");

        }
        public static bool RequiresPrettyPrint(this Normal norm)
        {
            return Printers.ContainsKey(norm.Head);
        }

        public static string PrettyPrint(this Normal norm)
        {
            try
            {
                var printer = Printers[norm.Head];
                return printer(norm);
            } 
            catch (KeyNotFoundException)
            {
                return norm.FullForm();
            }
        }

        private static string PrintOperator(Normal norm, string op)
        {
            StringBuilder bldr = new StringBuilder();
            bldr.Append(norm.First().PrettyForm());

            var args = norm.Skip(1);
            foreach (var arg in args)
            {
                bldr.Append(" " + op + " ");
                bldr.Append(arg.PrettyForm());
            }
            return bldr.ToString();
        }

        private static string PrintSum(Normal norm)
        {
            StringBuilder bldr = new StringBuilder();
            bldr.Append(norm.First().PrettyForm());

            var args = norm.Skip(1);
            foreach (var arg in args)
            {
                if (arg.Negative())
                {
                    bldr.Append(" - ");
                    bldr.Append(arg.AsNonnegative().PrettyForm());
                }
                else
                {
                    bldr.Append(" + ");
                    bldr.Append(arg.PrettyForm());
                }
            }
            return bldr.ToString();
        }
        private static string PrintProduct(Normal norm)
        {
            StringBuilder bldr = new StringBuilder();

            bldr.Append(ProductString(norm.First()));

            var args = norm.Skip(1);
            foreach (var arg in args)
            {
                if (arg.Head == "pow")
                {
                    Normal pow = arg as Normal;
                    if (pow[1].Negative())
                    {
                        Expression posPow = pow[1].AsNonnegative();
                        bldr.Append(" / ");
                        bldr.Append(PowString(pow[0]));
                        if (!posPow.Equals(new Integer(1, norm.Kernel)))
                        {
                            bldr.Append("^");
                            bldr.Append(PowString(posPow));
                        }
                    }
                    else
                    {
                        bldr.Append(" * ");
                        bldr.Append(ProductString(arg));
                    }
                }
                else
                {
                    bldr.Append(" * ");
                    bldr.Append(ProductString(arg));
                }
            }
            return bldr.ToString();
        }
        private static string ProductString(Expression exp)
        {
            if (exp.Head == "add")
            {
                return "(" + exp.PrettyForm() + ")";
            }
            return exp.PrettyForm();
        }

        private static string PrintPow(Normal norm)
        {
            StringBuilder bldr = new StringBuilder();

            bldr.Append(ProductString(norm.First()));

            var args = norm.Skip(1);
            foreach (var arg in args)
            {
                bldr.Append("^");
                bldr.Append(PowString(arg));
            }
            return bldr.ToString();
        }
        private static string PowString(Expression exp)
        {
            if (exp.Head == "add" || exp.Head == "multiply")
            {
                return "(" + exp.PrettyForm() + ")";
            }
            return exp.PrettyForm();
        }
    }
}

