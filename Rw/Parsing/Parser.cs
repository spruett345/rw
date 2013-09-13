using System;
using System.IO;
using System.Numerics;
using System.Collections.Generic;
using Rw.Matching;
using Rw.Parsing.Generated;
using Rw.Evaluation;
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

        public Program ParseProgram()
        {
            Node root = InternalParser.Parse();
            List<Rule> rules = new List<Rule>();
            List<Expression> expressions = new List<Expression>();
            for (int i = 0; i < root.GetChildCount(); i++)
            {
                Node child = root.GetChildAt(i);
                if (child.Id == (int)GrammarConstants.EXPRESSION 
                    || child.Id == (int)GrammarConstants.LET_EXPRESSION 
                    || child.Id == (int)GrammarConstants.IF_EXPRESSION)
                {
                    expressions.Add(ParseExpression(child));
                }
                else if (child.Id == (int)GrammarConstants.PATTERN_DEFINITION)
                {
                    Pattern p = ParsePattern(child.GetChildAt(1));
                   
                    int index = 3;
                    if (child.GetChildAt(2).Id == (int)GrammarConstants.WHERE)
                    {
                        Expression guard = ParseExpression(child.GetChildAt(3));
                        p = new GuardedPattern(guard, p);
                        index = 5;
                    }
                    Expression def = ParseExpression(child.GetChildAt(index));
                    Rule rule = new Rule(p, def);
                    rules.Add(rule);
                }
            }
            return new Program(rules, expressions);
        }

        public Expression ParseExpression()
        {
            return ParseExpression(InternalParser.Parse().GetChildAt(0));
        }
        public Pattern ParsePattern()
        {
            return ParsePattern(InternalParser.Parse().GetChildAt(0));
        }

        private Pattern ParsePattern(Node node)
        {
            if (node.Id == (int)GrammarConstants.FACTOR)
            {
                return ParseFactorPattern(node);
            }
            if (node.Id == (int)GrammarConstants.ATOM)
            {
                return ParseAtomPattern(node.GetChildAt(0));
            }
            return ParseBinaryPattern(node);
        }
        private Pattern ParseAtomPattern(Node node)
        {
            if (node.Id == (int)GrammarConstants.IDENTIFIER)
            {
                string id = ExtractValue(node);
                if (id == "true")
                {
                    return new LiteralPattern(new Boolean(true, Kernel));
                }
                if (id == "false")
                {
                    return new LiteralPattern(new Boolean(false, Kernel));
                }
                return new BoundPattern(new UntypedPattern(), ExtractValue(node));
            }
            if (node.Id == (int)GrammarConstants.NUMBER || node.Id == (int)GrammarConstants.DECIMAL)
            {
                string value = ExtractValue(node);
                if (value.Contains(".") || value.Contains("e") || value.Contains("E"))
                {
                    return new LiteralPattern(new Decimal(double.Parse(value), Kernel));
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
                
                if (tail.GetChildCount() > 2)
                {
                    var typeArgs = new List<string>();
                    for (int i = 3; i < tail.GetChildCount(); i += 2)
                    {
                        string val = ExtractValue(tail.GetChildAt(i));
                        typeArgs.Add(val);
                    }
                    if (type == "const")
                    {
                        return new ConstantPattern(typeArgs[0]);
                    }
                    if (type == "depends_on")
                    {
                        return new DependsOnPattern(typeArgs[0]);
                    }
                }
                return new BoundPattern(new TypedPattern(type), id);
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
        private Pattern ParseBinaryPattern(Node node)
        {
            if (node.GetChildCount() == 1)
            {
                return ParsePattern(node.GetChildAt(0));
            }
            else
            {
                Node tail = node.GetChildAt(1);
                string op = ExtractValue(tail.GetChildAt(0)).Trim();
                Node left = node.GetChildAt(0);
                Node right = tail.GetChildAt(1);
                return PatternOperate(op, left, right);
            }
        }

        private Pattern PatternOperate(string op, Node left, Node right)
        {
            switch (op)
            {
                case "+":
                    return new NormalPattern("add", true, ParsePattern(left), ParsePattern(right));
                case "*":
                    return new NormalPattern("multiply", true, ParsePattern(left), ParsePattern(right));
                case "^":
                    return new NormalPattern("pow", ParsePattern(left), ParsePattern(right));
                case "and":
                    return new NormalPattern("and", ParsePattern(left), ParsePattern(right));
                case "or":
                    return new NormalPattern("or", ParsePattern(left), ParsePattern(right));
                case ">":
                    return new NormalPattern("gt", ParsePattern(left), ParsePattern(right));
                case "<":
                    return new NormalPattern("lt", ParsePattern(left), ParsePattern(right));
                case ">=":
                    return new NormalPattern("gte", ParsePattern(left), ParsePattern(right));
                case "<=":
                    return new NormalPattern("lte", ParsePattern(left), ParsePattern(right));
                case "=":
                    return new NormalPattern("equals", ParsePattern(left), ParsePattern(right));
            }
            throw new Exception("Did not expect operator " + op  + " in pattern");
        }

        private Expression ParseAtom(Node node)
        {
            if (node.Id == (int)GrammarConstants.IDENTIFIER)
            {
                string value = ExtractValue(node);
                if (value == "true")
                {
                    return new Boolean(true, Kernel);
                }
                else if (value == "false")                    
                {
                    return new Boolean(false, Kernel);
                }
                return new Symbol(ExtractValue(node), Kernel);
            }
            if (node.Id == (int)GrammarConstants.NUMBER || node.Id == (int)GrammarConstants.DECIMAL)
            {
                string value = ExtractValue(node);
                if (value.Contains(".") || value.Contains("e") || value.Contains("E"))
                {
                    return new Decimal(double.Parse(value), Kernel);
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

                case ">":
                    return new Normal("gt", Kernel, ParseExpression(left), ParseExpression(right));
                case "<":
                    return new Normal("lt", Kernel, ParseExpression(left), ParseExpression(right));
                case ">=":
                    return new Normal("gte", Kernel, ParseExpression(left), ParseExpression(right));
                case "<=":
                    return new Normal("lte", Kernel, ParseExpression(left), ParseExpression(right));
                case "=":
                    return new Normal("equals", Kernel, ParseExpression(left), ParseExpression(right));

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
            if (node.Id == (int)GrammarConstants.LET_EXPRESSION)
            {
                string id = ExtractValue(node.GetChildAt(1));
                Expression value = ParseExpression(node.GetChildAt(3));
                if (node.GetChildAt(4).Id == (int)GrammarConstants.LET_LIST)
                {
                    Node list = node.GetChildAt(4);
                    var bindings = new Stack<Tuple<string, Expression>>();
                    bindings.Push(new Tuple<string, Expression>(id, value));
                    for (int i = 1; i < list.GetChildCount(); i += 4)
                    {
                        string name = ExtractValue(list.GetChildAt(i));
                        Expression val = ParseExpression(list.GetChildAt(i + 2));
                        bindings.Push(new Tuple<string, Expression>(name, val));
                    }
                    Expression bind = ParseExpression(node.GetChildAt(6));
                    var pop = bindings.Pop();
                    Bind outer = new Bind(new Symbol(pop.Item1, Kernel), pop.Item2, bind, Kernel);
                    while (bindings.Count > 0)
                    {
                        pop = bindings.Pop();
                        outer = new Bind(new Symbol(pop.Item1, Kernel), pop.Item2, outer, Kernel);
                    }
                    return outer;
                }
                else
                {
                    Expression bind = ParseExpression(node.GetChildAt(5));
                    return new Bind(new Symbol(id, Kernel), value, bind, Kernel);
                }
            }
            if (node.Id == (int)GrammarConstants.IF_EXPRESSION)
            {
                Expression cond = ParseExpression(node.GetChildAt(1));
                Expression t = ParseExpression(node.GetChildAt(3));
                Expression f = ParseExpression(node.GetChildAt(5));
                return new Normal("if", Kernel, cond, t, f);
            }
            return ParseBinary(node);
        }

        private string ExtractValue(Node node)
        {
            return Input[node.StartLine - 1].Substring(node.StartColumn - 1, node.EndColumn - node.StartColumn + 1);
        }
    }
}

