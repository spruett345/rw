using System;
using System.Linq;
using System.Collections.Generic;

namespace Rw
{
    public class Lambda : Normal
    {
        public readonly Symbol Parameter;
        public readonly Expression Body;

        public Lambda(Symbol param, Expression body, Kernel Kernel) : base("lambda", Kernel)
        {
            Parameter = param;
            Body = body;

            Variables.Remove(Parameter);
        }

        public override Normal Create(params Expression[] args)
        {
            if (args.Length != 2)
            {
                throw new Exception("Cannot create a lambda expression with less than two arguments (parameter, body)");
            }
            var sym = args[0] as Symbol;
            if (sym == null)
            {
                throw new Exception("Cannot create a lambda binding with a non-parameterized expression " + args[0].ToString());
            }
            return new Lambda(sym, args[1], Kernel);
        }
        public override Expression Apply(params Expression[] arguments)
        {
            if (arguments.Length == 0)
            {
                return this;
            }
            else if (arguments.Length == 1)
            {
                var env = new SubstitutionEnvironment();
                env.Bind(Parameter.Name, arguments[0]);
                return Body.Substitute(env);
            }
            else
            {
                var env = new SubstitutionEnvironment();
                env.Bind(Parameter.Name, arguments[0]);
                return Body.Substitute(env).Apply(arguments.Skip(1).ToArray());
            }
        }

        public override IEnumerator<Expression> GetEnumerator()
        {
            yield return Parameter;
            yield return Body;
        }

        public override string FullForm()
        {
            return "lambda(" + Parameter.FullForm() + " -> " + Body.FullForm() + ")";    
        }
        public override string PrettyForm()
        {
            return "(\\" + Parameter.PrettyForm() + " -> " + Body.PrettyForm() + ")";
        }
    }
}

