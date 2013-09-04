using System;

namespace Rw
{
    public abstract class Environment
    {
        public abstract bool ContainsKey(string key);

        public abstract Expression this[string key] { get; }
    }
}

