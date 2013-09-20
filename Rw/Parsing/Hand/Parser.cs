using System;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;

namespace Rw.Parsing.Hand
{
    public class Parser
    {
        private static Dictionary<string, int> OperatorPrecedence;
        private static Dictionary<string, string> OperatorMappings;

        private readonly Tokenizer Tokenizer;
        private readonly IEnumerator<Token> Tokens;
        private readonly Kernel Kernel;

        static Parser()
        {
            OperatorPrecedence = new Dictionary<string, int>();
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

            OperatorPrecedence["and"] = 0;
            OperatorPrecedence["or"] = 0;

            OperatorPrecedence["<"] = 1;
            OperatorPrecedence[">"] = 1;
            OperatorPrecedence["<="] = 1;
            OperatorPrecedence[">="] = 1;
            OperatorPrecedence["="] = 1;
            OperatorPrecedence["!="] = 1;

            OperatorPrecedence["+"] = 2;
            OperatorPrecedence["-"] = 2;
            OperatorPrecedence["*"] = 3;
            OperatorPrecedence["/"] = 3;
            OperatorPrecedence["^"] = 4;
            OperatorPrecedence["unaryminus"] = 10;
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
                start = Take();
            }
            if (start.Type == TokenType.Keyword)
            {
                if (start.Value == "let")
                {
                    return ParseLetExpression();
                }
                else if (start.Value == "if")
                {

                }
            }
            else
            {
                return ShuntingYard(start);
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

        private Expression ShuntingYard(Token start)
        {
            var operatorStack = new Stack<string>();
            var expressionStack = new Stack<Expression>();
            var last = new Token("", TokenType.None);

            var token = start;

            while (token.Type != TokenType.NewLine || (IsOperator(last) || operatorStack.Contains("(")))
            {
                var ins = OperatorInsert(last, token);
                if (ins != null)
                {
                    while (operatorStack.Count > 0 && OperatorPrecedence.ContainsKey(operatorStack.Peek()))
                    {
                        int comp = ComparePrecedence(ins, operatorStack.Peek());
                        if (comp < 0 || (token.Value != "^" && comp <= 0))
                        {
                            var expr = OperatorPop(operatorStack, expressionStack);
                            expressionStack.Push(expr);
                        }
                        else
                        {
                            break;
                        }
                    }
                    operatorStack.Push(ins);
                }
                if (token.Type == TokenType.End)
                {
                    break;
                }
                if (token.Type == TokenType.Decimal)
                {
                    var dec = new Decimal(double.Parse(token.Value), Kernel);
                    expressionStack.Push(dec);
                }
                else if (token.Type == TokenType.Integer)
                {
                    var integer = new Integer(BigInteger.Parse(token.Value), Kernel);
                    expressionStack.Push(integer);
                }
                else if (token.Value == "(")
                {
                    operatorStack.Push(token.Value);
                }
                else if (token.Value == ")")
                {
                    if (operatorStack.Count == 0)
                    {
                        throw new ParseException("mismatched parantheses, found extra ')'", 
                                                 Tokenizer.GetLineNumber());
                    }
                    while (operatorStack.Peek() != "(")
                    {
                        var exp = OperatorPop(operatorStack, expressionStack);

                        expressionStack.Push(exp);

                        if (operatorStack.Count == 0)
                        {
                            throw new ParseException("mismatched parantheses, found extra ')'", 
                                                    Tokenizer.GetLineNumber());
                        }
                    }
                    operatorStack.Pop();
                    if (operatorStack.Count > 0 && !OperatorPrecedence.ContainsKey(operatorStack.Peek()))
                    {
                        var func = FunctionPop(operatorStack, expressionStack);
                        expressionStack.Push(func);
                    }
                }
                else if (token.Value == ",")
                {
                    while (operatorStack.Count > 0 && operatorStack.Peek() != "(")
                    {
                        var exp = OperatorPop(operatorStack, expressionStack);
                        expressionStack.Push(exp);
                    }
                }
                else if (token.Type == TokenType.Identifier)
                {
                    string id = token.Value;
                    if (Peek().Value == "(")
                    {
                        expressionStack.Push(null);
                        operatorStack.Push(id);
                    }
                    else
                    {
                        var sym = new Symbol(id, Kernel);
                        expressionStack.Push(sym);
                    }
                }
                else if (IsOperator(token))
                {
                    if (token.Value == "-" && (last.Value == "(" || IsOperator(last)))
                    {
                        operatorStack.Push("unaryminus");
                    }
                    else
                    {
                        while (operatorStack.Count > 0 && OperatorPrecedence.ContainsKey(operatorStack.Peek()))
                        {
                            int comp = ComparePrecedence(token.Value, operatorStack.Peek());
                            if (comp < 0 || (token.Value != "^" && comp <= 0))
                            {
                                var expr = OperatorPop(operatorStack, expressionStack);
                                expressionStack.Push(expr);
                            }
                            else
                            {
                                break;
                            }
                        }
                        operatorStack.Push(token.Value);
                    }
                }
                else
                {
                    throw new ParseException("unhandled token, did not know what to do with token "
                        + token.Value + " [" + token.Type + "] in infix expression", 
                                             Tokenizer.GetLineNumber());
                }

                last = token;

                if (Peek().Type == TokenType.Keyword)
                {
                    break;
                }
                token = Take();
            }
            while (operatorStack.Count > 0)
            {
                if (operatorStack.Peek() == "(")
                {
                    throw new ParseException("could not find ending ')' to match starting '('", 
                                             Tokenizer.GetLineNumber());
                }
                expressionStack.Push(OperatorPop(operatorStack, expressionStack));
            }
            if (expressionStack.Count != 1)
            {
                throw new ParseException("not enough operators to apply on these expressions", Tokenizer.GetLineNumber());
            }
            return expressionStack.Pop();
        }

        private static int ComparePrecedence(string op1, string op2)
        {
            int prec1, prec2;
            if (!OperatorPrecedence.TryGetValue(op1, out prec1))
            {
                prec1 = 1000;
            }
            if (!OperatorPrecedence.TryGetValue(op2, out prec2))
            {
                prec2 = 1000;
            }
            return prec1 - prec2;
        }
        private Expression FunctionPop(Stack<string> opStack, Stack<Expression> exprStack)
        {
            string func = opStack.Pop();
            var args = new List<Expression>();

            while (exprStack.Peek() != null)
            {
                args.Add(exprStack.Pop());
            }

            exprStack.Pop();
            args.Reverse();

            return new Normal(func, Kernel, args.ToArray());
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

        private string OperatorInsert(Token left, Token right)
        {
            if (NumericToken(left) && NumericToken(right))
            {
                if (right.Value.StartsWith("-"))
                {
                    return "+";
                }
                return "*";
            }
            if (NumericToken(left))
            {
                if (NumericToken(right) || right.Type == TokenType.Identifier || right.Value == "(")
                {
                    return "*";
                }
            }
            if (left.Type == TokenType.Identifier)
            {
                if (NumericToken(right) || right.Type == TokenType.Identifier)
                {
                    return "*";
                }
            }
            return null;
        }

        private bool NumericToken(Token token)
        {
            return token.Type == TokenType.Decimal 
                || token.Type == TokenType.Integer;
        }

        private bool IsOperator(Token token)
        {
            return token.Type == TokenType.Symbol && OperatorPrecedence.ContainsKey(token.Value);
        }
        private Token Peek()
        {
            var enumerator = Tokens;
            return enumerator.Current;
        }
        private Token Take(bool ignoreNewlines = true)
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
    }
}

