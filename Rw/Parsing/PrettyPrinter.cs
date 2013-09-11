using System;
using System.Text;
using System.Linq;
namespace Rw.Parsing
{
    public static class PrettyPrinter
    {
        public static bool RequiresPrettyPrint(this Normal norm)
        {
            return norm.Head == "add" || norm.Head == "multiply" || norm.Head == "pow";
        }

        public static string PrettyPrint(this Normal norm)
        {
            if (norm.Head == "add")
            {
                return PrintSum(norm);
            }
            if (norm.Head == "multiply")
            {
                return PrintProduct(norm);
            }
            if (norm.Head == "pow")
            {
                return PrintPow(norm);
            }
            return norm.FullForm();
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

