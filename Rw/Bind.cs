using System;
using System.Collections.Generic;
using Rw.Evaluation;
namespace Rw
{
    /// <summary>
    /// Represents a variable binding into expression evaluation,
    /// such as expression of the form let x = 1 in x * x.
    /// </summary>
    public class Bind : Normal
    {
        private readonly Symbol Symbol;
        private readonly Expression Bound;
        private readonly Expression Value;

        public Bind(Symbol sym, Expression bind, Expression value, Kernel kernel) : base("bind", kernel)
        {
            Symbol = sym;
            Bound = bind;
            Value = value;
        }

        public override Normal Create(params Expression[] args)
        {
            if (args.Length != 3)
            {
                return base.Create(args);
            }
            return new Bind(args[0] as Symbol, args[1], args[2], Kernel);
        }
        public override IEnumerator<Expression> GetEnumerator()
        {
            yield return Symbol;
            yield return Bound;
            yield return Value;
        }

        public override Expression Substitute(Environment env)
        {
            if (!env.ContainsKey(Symbol.Name))
            {
                return new Bind(Symbol, Bound.Substitute(env), Value.Substitute(env), Kernel);
            }
            else
            {
                var environment = new SubstitutionEnvironment(env);
                environment.Unbind(Symbol.Name);
                return new Bind(Symbol, Bound.Substitute(environment), Value.Substitute(environment), Kernel);
            }
        }
        public override bool TryEvaluate(Lookup rules, out Expression evaluated)
        {
            Expression bind = Bound.Evaluate(rules);
            var environment = new SubstitutionEnvironment();
            environment.Bind(Symbol.Name, bind);

            evaluated = Value.Substitute(environment);
            return true;
        }

        public override ISet<Symbol> FreeVariables()
        {
            Variables.Remove(Symbol);
            return Variables;
        }

        public override string PrettyForm()
        {
            return "let " + Symbol.PrettyForm() + " = " + Bound.PrettyForm() + " in " + Value.PrettyForm();
        }
    }
}

