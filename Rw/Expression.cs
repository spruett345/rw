using System;
using System.Collections.Generic;
using Rw.Evaluation;

namespace Rw
{
    /// <summary>
    /// A base class for all Expression types.
    /// Defines an interface for methods to be implemented to express
    /// common functionality, and provides several default implementations.
    /// </summary>
    public abstract class Expression
    {
        protected HashSet<Symbol> Variables;

        /// <summary>
        /// Returns a head for the expression identifying its
        /// type or function name.
        /// </summary>
        public abstract string Head { get; }

        /// <summary>
        /// Returns a concrete type class for this expression,
        /// dividing expression into classes.
        /// </summary>
        public abstract TypeClass Type { get; }

        public readonly Kernel Kernel;

        public Expression(Kernel kernel)
        {
            Kernel = kernel;
            Variables = new HashSet<Symbol>();
        }

        /// <summary>
        /// Return whether this expression is decidedly negative or
        /// less than zero. True if this < 0, false if not or 
        /// undecidable.
        /// </summary>
        public virtual bool Negative()
        {
            return false;
        }

        /// <summary>
        /// Returns a new expression with negatives removed.
        /// </summary>
        /// <returns>
        /// The absolute value for numerics or a total
        /// absolute value for products.
        /// </returns>
        public virtual Expression AsNonnegative()
        {
            return this;
        }

        /// <summary>
        /// Returns whether this expression is numeric (int, decimal)
        /// or a numeric function with numeric arguments.
        /// </summary>
        public virtual bool Numeric()
        {
            return false;
        }

        /// <summary>
        /// Returns whether this expression contains
        /// decimal (floating point imprecise values).
        /// </summary>
        public virtual bool Imprecise()
        {
            return false;
        }
        /// <summary>
        /// Converts this expression to an imprecise
        /// object (all integers to decimals).
        /// </summary>
        /// <returns>
        /// A new expression in imprecise form.
        /// </returns>
        public virtual Expression AsImprecise()
        {
            return this;
        }

        /// <summary>
        /// Substitutes all variables mapped in the environment
        /// to this expression.
        /// </summary>
        /// <param name='env'>
        /// The environment which contains the mappings.
        /// </param>
        /// <returns>
        /// An expression identical to this with variables substituted.
        /// </returns>
        public virtual Expression Substitute(Environment env)
        {
            return this;
        }

        /// <summary>
        /// Evaluates this expression in the context of its Kernel
        /// by repeated application of rewrites. Evaluation only terminates
        /// when there are no more rules to rewrite this expression with.
        /// </summary>
        /// <param name='rules'>
        /// Option lookup of rules to evaluate with, if not the Kernel
        /// default rules are used.
        /// </param>
        public virtual Expression Evaluate(Lookup rules = null)
        {
            if (rules == null)
            {
                rules = Kernel.DefaultRules();
            }
            Expression evaluated = this;
            if (evaluated.Imprecise())
            {
                evaluated = evaluated.AsImprecise();
            }
            Expression prev = evaluated;
            while (prev.TryEvaluate(rules, out evaluated))
            {
                prev = evaluated;
            }
            return prev;
        }
        /// <summary>
        /// Tries one iteration of expression evaluation.
        /// </summary>
        /// <returns>
        /// True if a rewrite occurred false if no rules matched.
        /// </returns>
        /// <param name='rules'>
        /// Rules to attempt to rewrite with.
        /// </param>
        /// <param name='evaluated'>
        /// If a rewrite occurs, evaluated is set to the rewritten value.
        /// If no rewrite occurs this value is null.
        /// </param>
        public virtual bool TryEvaluate(Lookup rules, out Expression evaluated)
        {
            evaluated = null;
            return false;
        }

        /// <summary>
        /// Gives a set of variables contained within the expression.
        /// These are the set of variables that this expression
        /// depends on.
        /// </summary>
        /// <returns>
        /// The variables in a set.
        /// </returns>
        public virtual ISet<Symbol> FreeVariables()
        {
            return Variables;
        }

        /// <summary>
        /// Gives a full form representation of the expression
        /// in string form.
        /// </summary>
        public abstract string FullForm();

        /// <summary>
        /// Gives a pretty output with operators printed in
        /// infix notation as opposed to function.
        /// </summary>
        public virtual string PrettyForm()
        {
            return FullForm();
        }
        
        public override string ToString()
        {
            return PrettyForm();
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            return false;
        }
    }
}

