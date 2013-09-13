using System;
using System.Collections.Generic;
using System.Numerics;
using Rw.Matching;
using Rw.Evaluation;
using Rw.Parsing;

namespace Rw
{
    /// <summary>
    /// A kernel is an object which gives a context for expressions
    /// to be evaluated in. It contains rules and acts as an environment to
    /// store non expressiol-local information about execution.
    /// </summary>
    public class Kernel
    {
        public Dictionary<string, NormalAttributes> NormalAttributes;

        private Lookup BaseRules;
        private Lookup UserRules;

        private Environment Environment;

        public Kernel()
        {
            NormalAttributes = new Dictionary<string, Rw.NormalAttributes>();
            Environment = new SubstitutionEnvironment();
            BaseRules = new Lookup();
            UserRules = new Lookup();

            LoadAttributes();
            LoadHardRules();
        }

        /// <summary>
        /// Returns a list of the attributes of normal expressions
        /// with the specified head. These attributes inclde
        /// properties about functions/operators such as
        /// associative/commutative or protected functions.
        /// </summary>
        /// <returns>
        /// The normal attributes.
        /// </returns>
        /// <param name='head'>
        /// Head to search for attributes from.
        /// </param>
        public NormalAttributes GetNormalAttributes(string head)
        {
            NormalAttributes val = Rw.NormalAttributes.None;
            if (NormalAttributes.TryGetValue(head, out val))
            {
                return val;
            }
            return Rw.NormalAttributes.None;
        }

        public void Parse(string input, Func<Expression, Expression, bool> callback = null)
        {
            Parser parser = new Parser(input, this);
            Program program = parser.ParseProgram();

            foreach (Rule rule in program.Rules)
            {
                NormalPattern norm = rule.Pattern as NormalPattern;
                if (norm != null)
                {
                    UserRules.AddRule(norm.FunctionHead, rule);
                }
            }

            foreach (Expression exp in program.Expressions)
            {
                Expression eval = exp.Evaluate();
                if (callback != null)
                {
                    callback(exp, eval);
                }
            }
        }
        public void AddRule(string head, Rule rule)
        {
            BaseRules.AddRule(head, rule);
        }

        private void LoadAttributes()
        {
            NormalAttributes["add"] = Rw.NormalAttributes.Flat | 
                Rw.NormalAttributes.Numeric | Rw.NormalAttributes.Operator | Rw.NormalAttributes.Orderless
                    | Rw.NormalAttributes.Protected;
            NormalAttributes["multiply"] = Rw.NormalAttributes.Flat | 
                Rw.NormalAttributes.Numeric | Rw.NormalAttributes.Operator | Rw.NormalAttributes.Orderless
                    | Rw.NormalAttributes.Protected;

            NormalAttributes["pow"] = Rw.NormalAttributes.Operator | Rw.NormalAttributes.Numeric |
                Rw.NormalAttributes.Protected;

            NormalAttributes["log"] = Rw.NormalAttributes.Numeric | Rw.NormalAttributes.Protected;
            NormalAttributes["sin"] = Rw.NormalAttributes.Numeric | Rw.NormalAttributes.Protected;
            NormalAttributes["cos"] = Rw.NormalAttributes.Numeric | Rw.NormalAttributes.Protected;
            NormalAttributes["tan"] = Rw.NormalAttributes.Numeric | Rw.NormalAttributes.Protected;
        }
        private void LoadHardRules()
        {
            MakeHardRule("x:int + y:int", (env) =>
                         new Integer((env["x"] as Integer).Value + (env["y"] as Integer).Value, this));
            MakeHardRule("x:decimal + y:decimal", (env) =>
                         new Decimal((env["x"] as Decimal).Value + (env["y"] as Decimal).Value, this));

            MakeHardRule("x:int * y:int", (env) =>
                         new Integer((env["x"] as Integer).Value * (env["y"] as Integer).Value, this));
            MakeHardRule("x:decimal * y:decimal", (env) =>
                         new Decimal((env["x"] as Decimal).Value * (env["y"] as Decimal).Value, this));

            MakeHardRule("x:int ^ y:int", (env) => {
                Integer x = env["x"] as Integer;
                Integer y = env["y"] as Integer;

                if (y.Value < 0 || y.Value > int.MaxValue)
                {
                    return null;
                }
                return new Integer(BigInteger.Pow(x.Value, (int)y.Value), this);
            });
            MakeHardRule("x:decimal ^ y:decimal", (env) => {
                Decimal x = env["x"] as Decimal;
                Decimal y = env["y"] as Decimal;

                return new Decimal(Math.Pow(x.Value, y.Value), this);
            });

            MakeHardRule("log(x:decimal)", (env) =>
                         new Decimal(Math.Log((env["x"] as Decimal).Value), this));
            MakeHardRule("sin(x:decimal)", (env) =>
                         new Decimal(Math.Sin((env["x"] as Decimal).Value), this));
            MakeHardRule("cos(x:decimal)", (env) =>
                         new Decimal(Math.Cos((env["x"] as Decimal).Value), this));
        }
        private DefinedRule MakeHardRule(string pattern, Func<Environment, Expression> code)
        {
            Parser parser = new Parser(pattern + ";", this);
            Pattern pat = parser.ParsePattern();
            DefinedRule rule = new DefinedRule(pat, code);
            BaseRules.AddRule(GetRuleHead(rule), rule);
            return rule;
        }
        private string GetRuleHead(Rule rule)
        {
            NormalPattern norm = rule.Pattern as NormalPattern;
            if (norm != null)
            {
                return norm.FunctionHead;
            }
            throw new Exception("Could not extract normal head from pattern.");
        }

        /// <summary>
        /// Returns a default list of rules for expressions
        /// to be evaluated under given no manual value.
        /// </summary>
        /// <returns>
        /// The default rules, a union between the base and user
        /// defined rules.
        /// </returns>
        public Lookup DefaultRules()
        {
            return BaseRules.Union(UserRules);
        }
    }
}

