using System;
using System.Collections.Generic;

namespace Rw
{
    /// <summary>
    /// Implements an environment with a simple
    /// hashtable based dictionary, used for quick lookups.
    /// </summary>
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

        /// <summary>
        /// Binds the expression to a specific name or key.
        /// </summary>
        /// <param name='name'>
        /// Name or key to bind to.
        /// </param>
        /// <param name='value'>
        /// Value to be bound.
        /// </param>
        public void Bind(string name, Expression value)
        {
            Substitutions[name] = value;
        }
        /// <summary>
        /// Clears any bindings associated with the
        /// speciified name or key.
        /// </summary>
        /// <param name='name'>
        /// Name or key to clear bindings from.
        /// </param>
        public void Unbind(string name)
        {
            Substitutions.Remove(name);
        }
    }
}

