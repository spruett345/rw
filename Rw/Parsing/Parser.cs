using System;
using System.IO;
using System.Numerics;
using System.Collections.Generic;
using Rw.Matching;
using Rw.Parsing.Generated;
using PerCederberg.Grammatica.Runtime;

namespace Rw.Parsing
{
    public class Parser
    {
        private GrammarParser InternalParser;
        private Kernel Kernel;

        private string[] Input;

        public Parser(string input, Kernel kernel)
        {
            Input = input.Replace("\r\n", "\n").Split('\n');
            InternalParser = new GrammarParser(new StringReader(input));
            Kernel = kernel;
        }

        public void ParseProgram()
        {
            try
            {
                Node root = InternalParser.Parse();
                for (int i = 0; i < root.GetChildCount(); i++)
                {
                    Node child = root.GetChildAt(i);
                    if (child.Id == (int)GrammarConstants.COMPLEX_EXPRESSION)
                    {
                        Node inner = child.GetChildAt(0);
                        Console.WriteLine(ParseExpression(inner).Evaluate().PrettyForm());
                    }
                }
            } 
            catch (Exception e)
            {
                Console.WriteLine("Parser error: " + e.Message);
            }
        }

        public Expression ParseExpression()
        {
            return null;
        }
        public Pattern ParsePattern()
        {
            return null;
        }

        private Pattern ParsePattern(Node node)
        {
            return null;
        }
        private Pattern ParseAtomPattern(Node node)
        {
            if (node.Id == (int)GrammarConstants.IDENTIFIER)
            {
                return new BoundPattern(new UntypedPattern(), ExtractValue(node));
            }
            if (node.Id == (int)GrammarConstants.NUMBER || node.Id == (int)GrammarConstants.DECIMAL)
            {
                string value = ExtractValue(node);
                if (value.Contains(".") || value.Contains("e") || value.Contains("E"))
                {
                    return new LiteralPattern(new Decimal(decimal.Parse(value), Kernel));
                }
                return new LiteralPattern(new Integer(BigInteger.Parse(value), Kernel));
            }
            return null;
        }
        private Pattern ParseFactorPattern(Node node)
        {
            Node child = node.GetChildAt(0);
            if (child.Id == (int)GrammarConstants.ATOM)
            {
                return ParseAtomPattern(child.GetChildAt(0));
            }
            if (child.Id == (int)GrammarConstants.LPAR)
            {
                return ParsePattern(node.GetChildAt(1));
            }
            if (child.Id == (int)GrammarConstants.MINUS)
            {
                Pattern negated = ParsePattern(node.GetChildAt(1));
                //return new Normal("multiply", Kernel, negated, new Integer(-1, Kernel));
            }
            if (child.Id == (int)GrammarConstants.FUNCTION_EXPRESSION)
            {
                return ParseFunctionPattern(child);
            }
            if (child.Id == (int)GrammarConstants.IDENTIFIER)
            {
                string id = ExtractValue(child);
                Node tail = node.GetChildAt(1);
                string type = ExtractValue(tail.GetChildAt(1));
            }
            return null;
        }
        private Pattern ParseFunctionPattern(Node node)
        {
            Node id = node.GetChildAt(0);
            Node args = node.GetChildAt(2);
            if (args.Id != (int)GrammarConstants.RPAR)
            {
                Pattern[] arguments = new Pattern[args.GetChildCount() / 2 + 1];
                for (int i = 0; i < args.GetChildCount(); i += 2)
                {
                    arguments[i / 2] = ParsePattern(args.GetChildAt(i));
                }
                return new NormalPattern(ExtractValue(id), arguments);
            }
            else
            {
                return new NormalPattern(ExtractValue(id));
            }
        }

        private Expression ParseAtom(Node node)
        {
            if (node.Id == (int)GrammarConstants.IDENTIFIER)
            {
                return new Symbol(ExtractValue(node), Kernel);
            }
            if (node.Id == (int)GrammarConstants.NUMBER || node.Id == (int)GrammarConstants.DECIMAL)
            {
                string value = ExtractValue(node);
                if (value.Contains(".") || value.Contains("e") || value.Contains("E"))
                {
                    return new Decimal(decimal.Parse(value), Kernel);
                }
                return new Integer(BigInteger.Parse(value), Kernel);
            }
            return null;
        }
        private Expression ParseFactor(Node node)
        {
            Node child = node.GetChildAt(0);
            if (child.Id == (int)GrammarConstants.ATOM)
            {
                return ParseAtom(child.GetChildAt(0));
            }
            if (child.Id == (int)GrammarConstants.LPAR)
            {
                return ParseExpression(node.GetChildAt(1));
            }
            if (child.Id == (int)GrammarConstants.MINUS)
            {
                Expression negated = ParseExpression(node.GetChildAt(1));
                return new Normal("multiply", Kernel, negated, new Integer(-1, Kernel));
            }
            if (child.Id == (int)GrammarConstants.FUNCTION_EXPRESSION)
            {
                return ParseFunction(child);
            }
            if (child.Id == (int)GrammarConstants.IDENTIFIER)
            {
                throw new Exception("Did not expect type annotation in expression.");
            }
            return null;
        }
        private Expression ParseFunction(Node node)
        {
            Node id = node.GetChildAt(0);
            Node args = node.GetChildAt(2);
            if (args.Id != (int)GrammarConstants.RPAR)
            {
                Expression[] arguments = new Expression[args.GetChildCount() / 2 + 1];
                for (int i = 0; i < args.GetChildCount(); i += 2)
                {
                    arguments[i / 2] = ParseExpression(args.GetChildAt(i));
                }
                return new Normal(ExtractValue(id), Kernel, arguments);
            }
            else
            {
                return new Normal(ExtractValue(id), Kernel);
            }
        }
        private Expression ParseBinary(Node node)
        {
            if (node.GetChildCount() == 1)
            {
                return ParseExpression(node.GetChildAt(0));
            }
            else
            {
                Node tail = node.GetChildAt(1);
                string op = ExtractValue(tail.GetChildAt(0)).Trim();
                Node left = node.GetChildAt(0);
                Node right = tail.GetChildAt(1);
                return Operate(op, left, right);
            }
        }
        private Expression Operate(string op, Node left, Node right)
        {
            switch (op)
            {
                case "+":
                    return new Normal("add", Kernel, ParseExpression(left), ParseExpression(right));
                case "-":
                    return new Normal("add", Kernel,
                                      ParseExpression(left),
                                      new Normal("multiply", Kernel, new Integer(-1, Kernel), ParseExpression(right)));
                case "*":
                    return new Normal("multiply", Kernel, ParseExpression(left), ParseExpression(right));
                case "/":
                    return new Normal("multiply", Kernel,
                                      ParseExpression(left),
                                      new Normal("pow", Kernel, ParseExpression(right), new Integer(-1, Kernel)));
                case "^":
                    return new Normal("pow", Kernel, ParseExpression(left), ParseExpression(right));
                case "and":
                    return new Normal("and", Kernel, ParseExpression(left), ParseExpression(right));
                case "or":
                    return new Normal("or", Kernel, ParseExpression(left), ParseExpression(right));
            }
            throw new Exception("Unexpected operator " + op);
        }
        private Expression ParseExpression(Node node)
        {
            if (node.Id == (int)GrammarConstants.FACTOR)
            {
                return ParseFactor(node);
            }
            if (node.Id == (int)GrammarConstants.ATOM)
            {
                return ParseAtom(node.GetChildAt(0));
            }
            return ParseBinary(node);
        }

        private string ExtractValue(Node node)
        {
            return Input[node.StartLine - 1].Substring(node.StartColumn - 1, node.EndColumn - node.StartColumn + 1);
        }
    }
}

