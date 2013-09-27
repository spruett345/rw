using System;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;
using Rw.Matching;
using Rw.Evaluation;

namespace Rw.Parsing.Hand
{
    public class Parser
    {

        private static Dictionary<string, string> OperatorMappings;

        private readonly Tokenizer Tokenizer;
        private readonly IEnumerator<Token> Tokens;
        public readonly Kernel Kernel;

        static Parser()
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
        public Parser(string input, Kernel kernel)
        {
            Tokenizer = new Tokenizer(input);
            Tokens = Tokenizer.Tokens().GetEnumerator();
            Tokens.MoveNext();
            Kernel = kernel;
        }

        public Pattern ParsePattern(Token start = null)
        {
            if (start == null)
            {
                start = Peek();
            }
            var shuntingYard = new PatternParser(this);
            Pattern pattern;
            try
            {
                pattern = shuntingYard.Parse();
            }
            catch (ParseException ex)
            {
                if (ex.LineNumber)
                {
                    throw ex;
                }
                else
                {
                    throw new ParseException(ex.Message, Tokenizer.GetLineNumber());
                }
            }
            if (Peek().Value == "when")
            {
                Take();
                Expression condition = ParseExpression();

                pattern = new GuardedPattern(condition, pattern);
            }
            return pattern;
        }
        public Expression ParseExpression(Token start = null)
        {
            if (start == null)
            {
                start = Peek();
            }
            if (start.Type == TokenType.Keyword)
            {
                Take();
                if (start.Value == "let")
                {
                    return ParseLetExpression();
                }
                else if (start.Value == "if")
                {

                }
            }
            else if (start.Value == "\\")
            {
                return ParseLambdaExpression();
            }
            else
            {
                var shuntingYard = new ExpressionParser(this);
                try
                {
                    return shuntingYard.Parse();
                }
                catch (ParseException ex)
                {
                    if (ex.LineNumber)
                    {
                        throw ex;
                    }
                    else
                    {
                        throw new ParseException(ex.Message, Tokenizer.GetLineNumber());
                    }
                }
            }
            return null;
        }

        public Program ParseProgram()
        {
            Token token = Take();

            var rules = new List<Rule>();
            var expressions = new List<Expression>();

            while (token.Type != TokenType.End)
            {
                if (token.Value == "def")
                {
                    rules.Add(ParseRule());
                }
                else
                {
                    expressions.Add(ParseExpression(token));
                }
                token = Take();
            }
            return new Program(rules, expressions);
        }
        public Rule ParseRule(Token start = null)
        {
            if (start == null)
            {
                start = Peek();
            }

            Pattern pattern = ParsePattern(start);
            Expect(false, ":=");
            Expression body = ParseExpression();
            return new Rule(pattern, body);
        }
        private Token Expect(bool ignoreNewline, params TokenType[] type)
        {
            if (!type.Any(x => x == Peek(ignoreNewline).Type))
            {
                throw new ParseException("did not expect token of type " 
                                         + Peek().Type+  " (value " + Peek().Value + ")", 
                                         Tokenizer.GetLineNumber());
            }
            return Take(ignoreNewline);
        }
        private Token Expect(bool ignoreNewline, params string[] vals)
        {
            if (!vals.Any(x => x == Peek(ignoreNewline).Value))
            {
                throw new ParseException("did not expect token " + Peek().Value, 
                                         Tokenizer.GetLineNumber());
            }
            return Take(ignoreNewline);
        }

        private Token Expect(params TokenType[] type)
        {
            return Expect(true, type);
        }
        private Token Expect(params string[] vals)
        {
            return Expect(true, vals);
        }

        private Expression ParseLetExpression()
        {

            Token id = Expect(TokenType.Identifier);
            Expect("=");

            Expression value = ParseExpression();
            Token ex = Expect(",", "in");

            var stack = new Stack<Tuple<string, Expression>>();
            stack.Push(new Tuple<string, Expression>(id.Value, value));
            while (ex.Value == ",")
            {
                id = Expect(true, TokenType.Identifier);
                Expect("=");
                value = ParseExpression();

                stack.Push(new Tuple<string, Expression>(id.Value, value));
                ex = Expect(",", "in");
            }

            Expression bound = ParseExpression();
            var top = stack.Pop();

            var binding = new Bind(new Symbol(top.Item1, Kernel), top.Item2, bound, Kernel);
            while (stack.Count > 0)
            {
                top = stack.Pop();
                binding = new Bind(new Symbol(top.Item1, Kernel), top.Item2, binding, Kernel);
            }
            return binding;
        }
        private Expression ParseLambdaExpression()
        {
            if (Peek().Value == "\\")
            {
                Take();
            }

            Token id = Expect(TokenType.Identifier);

            Token exp = Expect(",", "->");

            var stack = new Stack<string>();
            stack.Push(id.Value);

            while (exp.Value == ",")
            {
                id = Expect(TokenType.Identifier);
                if (stack.Contains(id.Value))
                {
                    throw new ParseException("cannot create a lambda function with two variables that are the same");
                }
                stack.Push(id.Value);
                exp = Expect(",", "->");
            }
            Expression body = ParseExpression();
            Expression lambda = new Lambda(new Symbol(stack.Pop(), Kernel), body, Kernel);
            while (stack.Count > 0)
            {
                string v = stack.Pop();
                lambda = new Lambda(new Symbol(v, Kernel), lambda, Kernel);
            }
            return lambda;
        }



        private T FunctionPop<T>(Stack<string> opStack, Stack<T> exprStack,
                                 Func<string, IEnumerable<T>, T> createFunction)
        {
            string func = opStack.Pop();
            var args = new List<T>();

            while (exprStack.Peek() != null)
            {
                args.Add(exprStack.Pop());
            }

            exprStack.Pop();
            args.Reverse();

            return createFunction(func, args.ToArray());
        }
        private Expression OperatorPop(Stack<string> opStack, Stack<Expression> exprStack)
        {
            string op = opStack.Pop();

            if ((op == "-" && exprStack.Count == 1) || op == "unaryminus")
            {
                return new Normal(OperatorMappings["*"], Kernel, exprStack.Pop(), new Integer(-1, Kernel));
            }
            if (exprStack.Count < 2)
            {
                throw new ParseException("not enough operands to apply operator " + op, 
                                         Tokenizer.GetLineNumber());
            }
            Expression right = exprStack.Pop();
            Expression left = exprStack.Pop();
            if (OperatorMappings.ContainsKey(op))
            {
                return new Normal(OperatorMappings[op], Kernel, left, right);
            }
            if (op == "-")
            {
                return new Normal(OperatorMappings["+"], Kernel, left,
                                  new Normal(OperatorMappings["*"], Kernel, new Integer(-1, Kernel), right));
            }
            if (op == "/")
            {
                return new Normal(OperatorMappings["*"], Kernel, left,
                                  new Normal(OperatorMappings["^"], Kernel, right, new Integer(-1, Kernel)));
            }
            throw new ParseException("unknown operator " + op, Tokenizer.GetLineNumber());
        }

       
        public Token Take(bool ignoreNewlines = true)
        {
            if (ignoreNewlines)
            {
                while (Tokens.Current.Type == TokenType.NewLine)
                {
                    Tokens.MoveNext();
                }
            }
            Token take = Tokens.Current;
            Tokens.MoveNext();
            return take;
        }
        public Token Peek(bool ignoreNewlines = true)
        {
            if (ignoreNewlines)
            {
                while (Tokens.Current.Type == TokenType.NewLine)
                {
                    Tokens.MoveNext();
                }
            }
            Token take = Tokens.Current;
            return take;
        }
    }
}

