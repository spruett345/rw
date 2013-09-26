using System;
using System.Linq;
using System.Collections.Generic;

namespace Rw.Parsing.Hand
{
    public class ExpressionParser : ShuntingYardParser<Expression>
    {
        private readonly Parser Parser;

        private static Dictionary<string, string> OperatorMappings;

        static ExpressionParser()
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
        }

        public ExpressionParser(Parser parser) : base(parser.Peek, parser.Take, parser.Kernel)
        {
            Parser = parser;
        }

        protected override void HandleToken(Token token)
        {
            if (token.Value == "\\")
            {
                ExpressionStack.Push(Parser.ParseExpression(token));
            }
            else
            {
                base.HandleToken(token);
            }
        }
        protected override Expression CreateSymbol(string id)
        {
            return new Symbol(id, Parser.Kernel);
        }
        protected override Expression CreateLiteral(Expression literal)
        {
            return literal;
        }
        protected override Expression Operate(string op, IEnumerable<Expression> args)
        {
            if (op == "unary -")
            {
                var negated = args.First();
                return new Normal("multiply", Parser.Kernel, new Integer(-1, Parser.Kernel), negated);
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
            }
            else if (OperatorMappings.ContainsKey(op))
            {
                var left = args.First();
                var right = args.ElementAt(1);
                return new Normal(OperatorMappings[op], Kernel, left, right);
            }
            throw new NotImplementedException();
        }
        protected override Expression CreateFunction(Expression head, IEnumerable<Expression> args)
        {
            throw new NotImplementedException();
        }
    }
}

