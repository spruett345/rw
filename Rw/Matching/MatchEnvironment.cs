using System;
using System.Collections.Generic;

namespace Rw.Matching
{
    /// <summary>
    /// An enviroment for matching purposes.
    /// Lookup is linear but it allows for easy backtracking
    /// through the matching process.
    /// </summary>
    public class MatchEnvironment : Environment
    {
        private Stack<Tuple<BoundPattern, Expression>> Bindings;

        public MatchEnvironment()
        {
            Bindings = new Stack<Tuple<BoundPattern, Expression>>();
        }

        public override Expression this[string key]
        {
            get
            {
                foreach (var entry in Bindings)
                {
                    if (entry.Item1.Name == key)
                    {
                        return entry.Item2;
                    }
                }
                return null;
            }
        }
        public override IEnumerable<string> Keys()
        {
            foreach (var tuple in Bindings)
            {
                yield return tuple.Item1.Name;
            }
        }

        public override bool ContainsKey(string key)
        {
            return this[key] !=  null;
        }

        /// <summary>
        /// Gives the current state of the environment
        /// so that it may be backtracked to the state
        /// later.
        /// </summary>
        public int State()
        {
            return Bindings.Count;
        }
        /// <summary>
        /// Backtracks to a previously recorded state.
        /// </summary>
        /// <param name='state'>
        /// State to backtrack to
        /// </param>
        public void Revert(int state)
        {
            while (Bindings.Count > state)
            {
                Bindings.Pop();
            }
        }

        /// <summary>
        /// Binds an expression to a named pattern.
        /// </summary>
        /// <param name='key'>
        /// Name to bind to.
        /// </param>
        /// <param name='pattern'>
        /// Pattern to bind to.
        /// </param>
        /// <param name='exp'>
        /// Expression to bind.
        /// </param>
        public virtual bool Bind(string key, Pattern pattern, Expression exp)
        {
            if (ContainsKey(key))
            {
                return false;
            }
            Bindings.Push(new Tuple<BoundPattern, Expression>(
                new BoundPattern(pattern, key),
                exp));
            return true;
        }
    }
}

