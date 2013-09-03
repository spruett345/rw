using System;

namespace Rw.Matching
{
    /// <summary>
    /// Abstract base class for all patterns that
    /// are used in pattern matching.
    /// </summary>
    public abstract class Pattern
    {
        public abstract bool Matches(Expression exp, Environment env);
    }
}

