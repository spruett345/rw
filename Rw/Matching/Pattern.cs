using System;
using System.Collections.Generic;

namespace Rw.Matching
{
    /// <summary>
    /// Abstract base class for all patterns that
    /// are used in pattern matching.
    /// </summary>
    public abstract class Pattern
    {
        public readonly ISet<string> Variables;

        public Pattern()
        {
            Variables = new HashSet<string>();
        }

        /// <summary>
        /// Determines whether this pattern is a match
        /// to the specified expression in the specified
        /// environment.
        /// </summary>
        /// <param name='exp'>
        /// The expression to match to.
        /// </param>
        /// <param name='env'>
        /// The environment to match under.
        /// </param>
        /// <returns>
        /// True if the pattern matches, false if not.
        /// </returns>
        public abstract bool Matches(Expression exp, MatchEnvironment env);

        /// <summary>
        /// Determines whether this pattern is a partial
        /// match to a normal expression (usually under
        /// flat/orderless properties). If the expression
        /// cannot be matched partially it attempts a full match.
        /// </summary>
        /// <returns>
        /// True if a partial or full match occurs, false if not.
        /// </returns>
        /// <param name='exp'>
        /// Expression to match against.
        /// </param>
        /// <param name='env'>
        /// Environment to match under.
        /// </param>
        /// <param name='matched'>
        /// If partial matching occurs, matched is set to
        /// the part of the expression that is matched.
        /// </param>
        /// <param name='rest'>
        /// If partial matching occurs, rest is set to the
        /// part of the expression that is unmatched.
        /// </param>
        public virtual bool MatchesPartial(Expression exp, MatchEnvironment env, out Expression matched, out Expression rest)
        {
            matched = null;
            rest = null;
            if (Matches(exp, env))
            {
                matched = exp;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Attempts to bind the expression into the environment,
        /// to a specified name.
        /// </summary>
        /// <param name='exp'>
        /// Expression value to bind into the environment.
        /// </param>
        /// <param name='env'>
        /// Environment to bind into.
        /// </param>
        public virtual void Bind(Expression exp, MatchEnvironment env)
        {

        }

        /// <summary>
        /// Determines whether this pattern requires a lookahead
        /// to more tokens to determine if it can match.
        /// Examples include a depends on pattern in a pattern
        /// such as integrate(y : depends_on(x), x).
        /// </summary>
        /// <returns>
        /// True if lookahead is required, false if not.
        /// </returns>
        public virtual bool RequiresLookahead()
        {
            return false;
        }
    }
}

