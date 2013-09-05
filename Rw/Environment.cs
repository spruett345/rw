using System;
using System.Collections.Generic;

namespace Rw
{
    public abstract class Environment
    {
        public abstract bool ContainsKey(string key);

        public abstract Expression this[string key] { get; }
        public abstract IEnumerable<string> Keys();
    }
}

