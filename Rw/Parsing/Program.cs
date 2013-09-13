using System;
using System.Collections.Generic;
using Rw.Matching;
using Rw.Evaluation;

namespace Rw.Parsing
{
    public class Program
    {
        public readonly IEnumerable<Rule> Rules;
        public readonly IEnumerable<Expression> Expressions;

        public Program(IEnumerable<Rule> rules, IEnumerable<Expression> expressions)
        {
            Rules = rules;
            Expressions = expressions;
        }
    }
}

