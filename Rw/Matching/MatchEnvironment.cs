using System;
using System.Collections.Generic;

namespace Rw.Matching
{
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
        public override bool ContainsKey(string key)
        {
            return this[key] !=  null;
        }

        public int State()
        {
            return Bindings.Count;
        }
        public void Revert(int state)
        {
            while (Bindings.Count > state)
            {
                Bindings.Pop();
            }
        }

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

