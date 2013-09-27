using System;
using System.Numerics;
using System.Collections.Generic;

namespace Rw.Parsing.Hand
{
    public abstract class ShuntingYardParser<T> where T : class
    {
        protected static Dictionary<string, Operator> Operators;

        protected Stack<T> ExpressionStack;
        protected Stack<string> OperatorStack;

        protected Token LastToken;

        protected Func<Token> Peek;
        protected Func<Token> Take;

        protected Kernel Kernel;

        static ShuntingYardParser()
        {
            Operators = new Dictionary<string, Operator>();

            AddOperator("and", 0);
            AddOperator("or", 0);

            AddOperator("<", 1);
            AddOperator(">", 1);
            AddOperator("<=", 1);
            AddOperator(">=", 1);
            AddOperator("=", 1);
            AddOperator("!=", 1);

            AddOperator("+", 2);
            AddOperator("-", 2);
            AddOperator("*", 3);
            AddOperator("/", 3);

            AddOperator("^", 4, true);
            AddOperator("unary -", 10, false, true);
            AddOperator("apply", 100);
        }
        static void AddOperator(string name, int prec, bool right = false, bool unary = false)
        {
            Operators[name] = new Operator(name, prec, right, unary);
        }

        public ShuntingYardParser(Func<Token> peek, Func<Token> take, Kernel kernel)
        {
            Kernel = kernel;
            Peek = peek;
            Take = take;

            LastToken = new Token("", TokenType.None);

            ExpressionStack = new Stack<T>();
            OperatorStack = new Stack<string>();
        }

        public T Parse()
        {
            if (Peek().Type == TokenType.End)
            {
                throw new ParseException("unexpected end when attempting to parse expression");
            }
            while (ContinueParsing())
            {
                var token = Take();
                string insert;
                if (InsertOperator(token, out insert))
                {
                    PopWhile(() => 
                             IsOperator(OperatorStack.Peek()) && CompareOperators(insert, OperatorStack.Peek()));
                    OperatorStack.Push(insert);
                }
                HandleToken(token);
                LastToken = token;
            }
            PopWhile(() => {
                if (OperatorStack.Peek() == "(")
                {
                    throw new ParseException("mismatched '(', did not find a closing ')'");
                }
                return true;
            }
            );
            if (ExpressionStack.Count != 1)
            {
                throw new ParseException("not enough operators left to apply to expressions");
            }
            return ExpressionStack.Pop();
        }

        protected virtual void HandleToken(Token token)
        {
            if (token.Type == TokenType.Decimal)
            {
                var d = double.Parse(token.Value);
                var dec = new Decimal(d, Kernel);

                ExpressionStack.Push(CreateLiteral(dec));
            }
            else if (token.Type == TokenType.Integer)
            {
                var i = BigInteger.Parse(token.Value);
                var integer = new Integer(i, Kernel);

                ExpressionStack.Push(CreateLiteral(integer));
            }
            else if (token.Type == TokenType.Identifier)
            {
                if (Peek().Value == "(")
                {
                    OperatorStack.Push(token.Value);
                    ExpressionStack.Push(default(T));
                }
                else
                {
                    var sym = CreateSymbol(token.Value);
                    ExpressionStack.Push(sym);
                }
            }
            else if (token.Value == ",")
            {
                PopWhile(() => OperatorStack.Peek() != "(");
            }
            else if (token.Value == "(")
            {
                OperatorStack.Push("(");
            }
            else if (token.Value == ")")
            {
                PopWhile(() => OperatorStack.Peek() != "(");
                if (OperatorStack.Count == 0)
                {
                    throw new ParseException("mismatched parantheses, found extra ')'");
                }
                OperatorStack.Pop();
                if (OperatorStack.Count > 0 && !IsOperator(OperatorStack.Peek()))
                {
                    ExpressionStack.Push(PopFunction());
                }
            }
            else if (IsOperator(token.Value))
            {
                if (token.Value == "-" && (LastToken.Value == "(" || IsOperator(LastToken.Value) || LastToken.Type == TokenType.None))
                {
                    OperatorStack.Push("unary -");
                }
                else
                {
                    string op = token.Value;
                    PopWhile(() => 
                             IsOperator(OperatorStack.Peek()) && CompareOperators(op, OperatorStack.Peek()));
                    OperatorStack.Push(op);
                }
            }
            else
            {
                throw new ParseException("unhandled token, did not know what to do with token "
                                         + token.Value + " [" + token.Type + "] in infix expression");
            }

        }

        protected virtual bool ContinueParsing()
        {
            if (Peek().Type == TokenType.End)
            {
                return false;
            }
            if (Peek().Value == ":=")
            {
                return false;
            }
            if (Peek().Value== ")")
            {
                return OperatorStack.Contains("(");
            }
            if (Peek().Type == TokenType.NewLine)
            {
                Take();
                if (!OperatorStack.Contains("("))
                {
                    return false;
                }
            }
            if (Peek().Value == ",")
            {
                return OperatorStack.Contains("(");
            }
            if (Peek().Value == ";")
            {
                Take();
                return false;
            }
            if (Peek().Type == TokenType.Keyword)
            {
                return false;
            }
            return true;
        }

        private bool InsertOperator(Token current, out string op)
        {
            if (current.Value == "(")
            {
                if (LastToken.Value == ")")
                {
                    ExpressionStack.Push(null);
                    op = "apply";
                    return true;
                }
            }
            if (NumericToken(LastToken) && NumericToken(current))
            {
                if (current.Value.StartsWith("-"))
                {
                    op = "+";
                    return true;
                }
                op = "*";
                return true;
            }
            if (NumericToken(LastToken))
            {
                if (NumericToken(current) || current.Type == TokenType.Identifier || current.Value == "(")
                {
                    op = "*";
                    return true;
                }
            }
            if (LastToken.Type == TokenType.Identifier)
            {
                if (NumericToken(current) || current.Type == TokenType.Identifier)
                {
                    op = "*";
                    return true;
                }
            }
            op = null;
            return false;
        }
        private bool IsOperator(string val)
        {
            return Operators.ContainsKey(val);
        }

        private bool CompareOperators(string left, string right)
        {
            Operator leftOp = Operators[left];
            Operator rightOp = Operators[right];

            return (leftOp.Precedence == rightOp.Precedence && !leftOp.RightAssociative)
                || leftOp.Precedence < rightOp.Precedence;
        }
        private void PopWhile(Func<bool> cond)
        {
            while (OperatorStack.Count > 0 && cond())
            {
                var exp = PopOperator();
                ExpressionStack.Push(exp);
            }
        }
        private bool NumericToken(Token token)
        {
            return token.Type == TokenType.Decimal 
                || token.Type == TokenType.Integer;
        }


        protected virtual T PopOperator()
        {
            var op = Operators[OperatorStack.Pop()];

            if (op.Value == "apply")
            {
                var args = new List<T>();
                while (ExpressionStack.Peek() != null)
                {
                    args.Add(ExpressionStack.Pop());
                }
                ExpressionStack.Pop();
                args.Reverse();

                var head = ExpressionStack.Pop();
                return CreateFunction(head, args);
            }
            else if (op.Unary)
            {
                if (ExpressionStack.Count < 1)
                {
                    throw new ParseException("expected at least one expression for unary operator '" + op.Value + "'");
                }

                return Operate(op.Value, new T[] { ExpressionStack.Pop() });
            }
            else
            {
                if (ExpressionStack.Count < 2)
                {
                    throw new ParseException("expected two expressions for operator '" + op.Value + "'");
                }

                var right = ExpressionStack.Pop();
                var left = ExpressionStack.Pop();
                return Operate(op.Value, new T[] { left, right });
            }
        }
        protected virtual T PopFunction()
        {
            var head = OperatorStack.Pop();
            var args = new List<T>();
            while (ExpressionStack.Peek() != null)
            {
                args.Add(ExpressionStack.Pop());
            }
            ExpressionStack.Pop();
            args.Reverse();
            return CreateFunction(CreateSymbol(head), args);
        }

        protected abstract T CreateSymbol(string id);
        protected abstract T CreateLiteral(Expression literal);
        protected abstract T CreateFunction(T head, IEnumerable<T> args);
        protected abstract T Operate(string op, IEnumerable<T> args);

    }
}

