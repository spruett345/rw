using System;
using System.Collections.Generic;
using System.Numerics;
using Rw.Matching;
using Rw.Evaluation;
using Rw.Parsing;
using Rw.Parsing.Hand;


namespace Rw
{
    using Parser = Rw.Parsing.Hand.Parser;
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

        private SubstitutionEnvironment KernelEnvironment;
        private SubstitutionEnvironment UserEnvironment;

        public Kernel()
        {
            NormalAttributes = new Dictionary<string, Rw.NormalAttributes>();
            KernelEnvironment = new SubstitutionEnvironment();
            UserEnvironment = new SubstitutionEnvironment();

            BaseRules = new Lookup();
            UserRules = new Lookup();

            LoadAttributes();
            LoadHardRules();
            LoadSymbolicConstants();
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
                Pattern pattern = rule.Pattern;
                GuardedPattern guarded =  pattern as GuardedPattern;
                if (guarded != null)
                {
                    pattern = guarded.BasePattern;
                }
                NormalPattern norm = pattern as NormalPattern;
                if (norm != null)
                {
                    UserRules.AddRule(norm.FunctionHead, rule);
                }
                else
                {
                    throw new Exception("Invalid pattern defined");
                }
            }

            foreach (Expression exp in program.Expressions)
            {
                Expression eval = Evaluate(exp);
                if (callback != null)
                {
                    callback(exp, eval);
                }
            }
        }
        public Expression Evaluate(Expression exp)
        {
            return exp.Substitute(KernelEnvironment).Substitute(UserEnvironment).Evaluate(DefaultRules());
        }

        public void AddRule(string head, Rule rule)
        {
            UserRules.AddRule(head, rule);
        }

        private void LoadSymbolicConstants()
        {
            KernelEnvironment.Bind("pi", new SymbolicConstant("pi", Math.PI, this));
            KernelEnvironment.Bind("e", new SymbolicConstant("e", Math.E, this));
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

            MakeHardRule("x:int > y:int", (env) =>
                         new Boolean((env["x"] as Integer).Value > (env["y"] as Integer).Value, this));
            MakeHardRule("x:decimal > y:decimal", (env) =>
                         new Boolean((env["x"] as Decimal).Value > (env["y"] as Decimal).Value, this));
            MakeHardRule("x:int < y:int", (env) =>
                         new Boolean((env["x"] as Integer).Value < (env["y"] as Integer).Value, this));
            MakeHardRule("x:decimal < y:decimal", (env) =>
                         new Boolean((env["x"] as Decimal).Value < (env["y"] as Decimal).Value, this));
            MakeHardRule("x:int >= y:int", (env) =>
                         new Boolean((env["x"] as Integer).Value >= (env["y"] as Integer).Value, this));
            MakeHardRule("x:decimal >= y:decimal", (env) =>
                         new Boolean((env["x"] as Decimal).Value >= (env["y"] as Decimal).Value, this));
            MakeHardRule("x:int <= y:int", (env) =>
                         new Boolean((env["x"] as Integer).Value <= (env["y"] as Integer).Value, this));
            MakeHardRule("x:decimal <= y:decimal", (env) =>
                         new Boolean((env["x"] as Decimal).Value <= (env["y"] as Decimal).Value, this));
            MakeHardRule("x:int = y:int", (env) =>
                         new Boolean((env["x"] as Integer).Value == (env["y"] as Integer).Value, this));
            MakeHardRule("x:decimal = y:decimal", (env) =>
                         new Boolean((env["x"] as Decimal).Value == (env["y"] as Decimal).Value, this));

            MakeHardRule("cons(x, y:list)", (env) =>
                         (env["y"] as List).Prepend(env["x"]));
            MakeHardRule("x:int ^ y:int", (env) => {
                Integer x = env["x"] as Integer;
                Integer y = env["y"] as Integer;

                if (y.Value < 0 && y.Value > int.MinValue)
                {
                    int pow = (int)y.Value * -1;
                    BigInteger val = BigInteger.Pow(x.Value, pow);
                    return new Rational(1, val, this);
                }
                else if (y.Value < 0 || y.Value > int.MaxValue)
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

            MakeHardRule("n(x)", (env) => (env["x"].AsImprecise()));
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

