using System;
using System.Linq;
using System.Collections.Generic;
using Rw.Matching;

namespace Rw.Parsing.Hand
{
    public class PatternParser : ShuntingYardParser<Pattern>
    {
        private readonly Parser Parser;

        private static Dictionary<string, string> OperatorMappings;

        static PatternParser()
        {
            OperatorMappings = new Dictionary<string, string>();

            OperatorMappings["+"] = "add";
            OperatorMappings["*"] = "multiply";
            OperatorMappings["^"] = "pow";
            OperatorMappings["<"] = "lt";
            OperatorMappings[">"] = "gt";
            OperatorMappings["<="] = "lte";
            OperatorMappings[">="] = "gte";
            OperatorMappings["="] = "eq";
            OperatorMappings["!="] = "neq";
            OperatorMappings["and"] = "and";
            OperatorMappings["or"] = "or";
        }

        public PatternParser(Parser parser) : base(() => parser.Peek(false), () => parser.Take(false), parser.Kernel)
        {
            Parser = parser;
        }

        protected override Pattern CreateSymbol(string id)
        {
            if (Peek().Value == ":")
            {
                Take();
                string head = Take().Value;
                if (Peek().Value == "(")
                {
                    Take();
                    var args = new List<string>();
                    while (Peek().Value != ")")
                    {
                        Token token = Take();
                        if (token.Type != TokenType.Identifier)
                        {
                            throw new ParseException("did not expect anything but identifier in pattern");
                        }

                        args.Add(token.Value);
                        if (Peek().Value == ",")
                        {
                            Take();
                        }
                        else if (Peek().Value != ")")
                        {
                            throw new Exception("expected ',' after identifier token");
                        }
                    }
                    Take();
                    return new BoundPattern(SpecialPattern(head, args), id);
                }
                return new BoundPattern(new TypedPattern(head), id);
            }
            return new BoundPattern(new UntypedPattern(), id);
        }
        private Pattern SpecialPattern(string head, IEnumerable<string> args)
        {

            if (head == "const")
            {
                if (args.Count() != 1)
                {
                    throw new ParseException("only special patterns of one argument exist at this time");
                }
                return new ConstantPattern(args.First());
            }
            else if (head == "depends_on")
            {
                if (args.Count() != 1)
                {
                    throw new ParseException("only special patterns of one argument exist at this time");
                }
                return new DependsOnPattern(args.First());
            }
            else if (head == "rational")
            {
                if (args.Count() != 2)
                {
                    throw new ParseException("rational pattern takes two arguments");
                }
                return new RationalPattern(args.ElementAt(0), args.ElementAt(1));
            }
            throw new ParseException("undefined special pattern with head " + head);
        }
        protected override Pattern CreateLiteral(Expression literal)
        {
            return new LiteralPattern(literal);
        }
        protected override Pattern Operate(string op, IEnumerable<Pattern> args)
        {
            /*if (op == "unary -")
            {
                var negated = args.First();
                return new NormalPattern("multiply", Parser.Kernel, new Integer(-1, Parser.Kernel), negated);
            }
            else if (op == "/")
            {
                var left = args.First();
                var right = args.ElementAt(1);
                return new Normal("multiply", Parser.Kernel, left, 
                                  new Normal("pow", Parser.Kernel, right, new Integer(-1, Parser.Kernel)));
            }
            else if (op == "-")
            {
                var left = args.First();
                var right = args.ElementAt(1);
                return new Normal("add", Parser.Kernel, left, 
                                  new Normal("multiply", Parser.Kernel, right, new Integer(-1, Parser.Kernel)));
            }*/
            if (OperatorMappings.ContainsKey(op))
            {
                var left = args.First();
                var right = args.ElementAt(1);
                var head = OperatorMappings[op];
                if (Parser.Kernel.GetNormalAttributes(head).HasFlag(NormalAttributes.Flat))
                {
                    return new NormalPattern(OperatorMappings[op], true, left, right);
                }
                return new NormalPattern(OperatorMappings[op], left, right);
            }
            throw new ParseException("unknown operator " + op + " in pattern");
        }
        protected override Pattern CreateFunction(Pattern head, IEnumerable<Pattern> args)
        {
            BoundPattern bound = head as BoundPattern;
            if (bound == null)
            {
                throw new ParseException("expected identifier to start function in pattern");
            }
            return new NormalPattern(bound.Name, args.ToArray());
        }
    }
}

