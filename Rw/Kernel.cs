using System;
using System.Collections.Generic;
using Rw.Matching;
using Rw.Evaluation;

namespace Rw
{
    /// <summary>
    /// A kernel is an object which gives a context for expressions
    /// to be evaluated in. It contains rules and acts as an environment to
    /// store non expressiol-local information about execution.
    /// </summary>
    public class Kernel
    {
        public Dictionary<string, NormalAttributes> NormalAttributes;

        private Lookup BaseRules;
        private Lookup UserRules;

        public Kernel()
        {
            NormalAttributes = new Dictionary<string, Rw.NormalAttributes>();
            BaseRules = new Lookup();
            UserRules = new Lookup();
        }

        /// <summary>
        /// Returns a list of the attributes of normal expressions
        /// with the specified head. These attributes inclde
        /// properties about functions/operators such as
        /// associative/commutative or protected functions.
        /// </summary>
        /// <returns>
        /// The normal attributes.
        /// </returns>
        /// <param name='head'>
        /// Head to search for attributes from.
        /// </param>
        public NormalAttributes GetNormalAttributes(string head)
        {
            NormalAttributes val = Rw.NormalAttributes.None;
            if (NormalAttributes.TryGetValue(head, out val))
            {
                return val;
            }
            return Rw.NormalAttributes.None;
        }
        
        public void AddRule(string head, Rule rule)
        {
            BaseRules.AddRule(head, rule);
        }

        /// <summary>
        /// Returns a default list of rules for expressions
        /// to be evaluated under given no manual value.
        /// </summary>
        /// <returns>
        /// The default rules, a union between the base and user
        /// defined rules.
        /// </returns>
        public Lookup DefaultRules()
        {
            return BaseRules.Union(UserRules);
        }
    }
}

