using System;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;

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

        private Token Expect(params TokenType[] type)
        {
            if (!type.Any(x => x == Peek().Type))
            {
                throw new ParseException("did not expect token of type " 
                                         + Peek().Type+  " (value " + Peek().Value + ")", 
                                         Tokenizer.GetLineNumber());
            }
            return Take();
        }
        private Token Expect(params string[] vals)
        {
            if (!vals.Any(x => x == Peek().Value))
            {
                throw new ParseException("did not expect token " + Peek().Value, 
                                         Tokenizer.GetLineNumber());
            }
            return Take();
        }
        private Expression ParseLetExpression()
        {

            Token id = Expect(TokenType.Identifier);
            Expect("=");

            Expression value = ParseExpression();
            Expect("in");
            Expression val = ParseExpression();
            return new Bind(new Symbol(id.Value, Kernel), value, val, Kernel);
        }
        private Expression ParseLambdaExpression()
        {
            if (Peek().Value == "\\")
            {
                Take();
            }

            Token id = Expect(TokenType.Identifier);
            Expect("->");
            Expression body = ParseExpression();

            return new Lambda(new Symbol(id.Value, Kernel), body, Kernel);
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

