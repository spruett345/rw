using System;
using System.Collections.Generic;

namespace Rw
{
    public class SubstitutionEnvironment : Environment
    {
        private Dictionary<string, Expression> Substitutions;

        public SubstitutionEnvironment()
        {
            Substitutions = new Dictionary<string, Expression>();
        }
        public SubstitutionEnvironment(Environment other)
        {
            Substitutions = new Dictionary<string, Expression>();

            foreach (string key in other.Keys())
            {
                Bind(key, other[key]);
            }
        }

        public override bool ContainsKey(string key)
        {
            return Substitutions.ContainsKey(key);
        }
        public override Expression this[string key]
        {
            get
            {
                return Substitutions[key];
            }
        }
        public override IEnumerable<string> Keys()
        {
            return Substitutions.Keys;
        }

        public void Bind(string name, Expression value)
        {
            Substitutions[name] = value;
        }
        public void Unbind(string name)
        {
            Substitutions.Remove(name);
        }
    }
}

