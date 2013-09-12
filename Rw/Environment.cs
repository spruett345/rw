using System;
using System.Collections.Generic;

namespace Rw
{
    /// <summary>
    /// A base class for environments to subsitute variables from.
    /// Acts as a map between symbol name and the expression
    /// to replace with.
    /// </summary>
    public abstract class Environment
    {
        /// <summary>
        /// Determines whether this environment
        /// contains a binding with the specific
        /// name.
        /// </summary>
        /// <param name='key'>
        /// Name to search for in the bindings.
        /// </params>
        /// <returns>
        /// True if mapping is found, false if not
        /// </returns>
        public abstract bool ContainsKey(string key);

        /// <summary>
        /// Gets the expression associated with the
        /// specified name binding.
        /// </summary>
        /// <param name='key'>
        /// Name to search for.
        /// </param>
        /// <returns>
        /// Bound expression if found, null if not found.
        /// </returns>
        public abstract Expression this[string key] { get; }

        /// <summary>
        /// Gets an enumarable of the keys bound to this
        /// environment so that they may be iterated over
        /// to retrieve the complete mapping.
        /// </summary>
        public abstract IEnumerable<string> Keys();
    }
}

