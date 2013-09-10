/*
 * GrammarParser.cs
 *
 * THIS FILE HAS BEEN GENERATED AUTOMATICALLY. DO NOT EDIT!
 */

using System.IO;

using PerCederberg.Grammatica.Runtime;

namespace Rw.Parsing.Generated {

    /**
     * <remarks>A token stream parser.</remarks>
     */
    internal class GrammarParser : RecursiveDescentParser {

        /**
         * <summary>An enumeration with the generated production node
         * identity constants.</summary>
         */
        private enum SynteticPatterns {
            SUBPRODUCTION_1 = 3001
        }

        /**
         * <summary>Creates a new parser with a default analyzer.</summary>
         *
         * <param name='input'>the input stream to read from</param>
         *
         * <exception cref='ParserCreationException'>if the parser
         * couldn't be initialized correctly</exception>
         */
        public GrammarParser(TextReader input)
            : base(input) {

            CreatePatterns();
        }

        /**
         * <summary>Creates a new parser.</summary>
         *
         * <param name='input'>the input stream to read from</param>
         *
         * <param name='analyzer'>the analyzer to parse with</param>
         *
         * <exception cref='ParserCreationException'>if the parser
         * couldn't be initialized correctly</exception>
         */
        public GrammarParser(TextReader input, GrammarAnalyzer analyzer)
            : base(input, analyzer) {

            CreatePatterns();
        }

        /**
         * <summary>Creates a new tokenizer for this parser. Can be overridden
         * by a subclass to provide a custom implementation.</summary>
         *
         * <param name='input'>the input stream to read from</param>
         *
         * <returns>the tokenizer created</returns>
         *
         * <exception cref='ParserCreationException'>if the tokenizer
         * couldn't be initialized correctly</exception>
         */
        protected override Tokenizer NewTokenizer(TextReader input) {
            return new GrammarTokenizer(input);
        }

        /**
         * <summary>Initializes the parser by creating all the production
         * patterns.</summary>
         *
         * <exception cref='ParserCreationException'>if the parser
         * couldn't be initialized correctly</exception>
         */
        private void CreatePatterns() {
            ProductionPattern             pattern;
            ProductionPatternAlternative  alt;

            pattern = new ProductionPattern((int) GrammarConstants.COMPLEX_EXPRESSION,
                                            "ComplexExpression");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int) GrammarConstants.EXPRESSION, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int) GrammarConstants.LET_EXPRESSION, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int) GrammarConstants.IF_EXPRESSION, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) GrammarConstants.EXPRESSION,
                                            "Expression");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int) GrammarConstants.BOOLEAN_EXPRESSION, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) GrammarConstants.LET_EXPRESSION,
                                            "LetExpression");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) GrammarConstants.LET, 1, 1);
            alt.AddToken((int) GrammarConstants.IDENTIFIER, 1, 1);
            alt.AddToken((int) GrammarConstants.EQ, 1, 1);
            alt.AddProduction((int) GrammarConstants.COMPLEX_EXPRESSION, 1, 1);
            alt.AddProduction((int) GrammarConstants.LET_LIST, 0, -1);
            alt.AddToken((int) GrammarConstants.IN, 1, 1);
            alt.AddProduction((int) GrammarConstants.COMPLEX_EXPRESSION, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) GrammarConstants.LET_LIST,
                                            "LetList");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) GrammarConstants.COMMA, 1, 1);
            alt.AddToken((int) GrammarConstants.IDENTIFIER, 1, 1);
            alt.AddToken((int) GrammarConstants.EQ, 1, 1);
            alt.AddProduction((int) GrammarConstants.COMPLEX_EXPRESSION, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) GrammarConstants.IF_EXPRESSION,
                                            "IfExpression");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) GrammarConstants.IF, 1, 1);
            alt.AddProduction((int) GrammarConstants.EXPRESSION, 1, 1);
            alt.AddToken((int) GrammarConstants.THEN, 1, 1);
            alt.AddProduction((int) GrammarConstants.EXPRESSION, 1, 1);
            alt.AddToken((int) GrammarConstants.ELSE, 1, 1);
            alt.AddProduction((int) GrammarConstants.EXPRESSION, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) GrammarConstants.FUNCTION_EXPRESSION,
                                            "FunctionExpression");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) GrammarConstants.IDENTIFIER, 1, 1);
            alt.AddToken((int) GrammarConstants.LPAR, 1, 1);
            alt.AddProduction((int) GrammarConstants.ARGUMENT_LIST, 0, 1);
            alt.AddToken((int) GrammarConstants.RPAR, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) GrammarConstants.ARGUMENT_LIST,
                                            "ArgumentList");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int) GrammarConstants.EXPRESSION, 1, 1);
            alt.AddProduction((int) SynteticPatterns.SUBPRODUCTION_1, 0, -1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) GrammarConstants.BOOLEAN_EXPRESSION,
                                            "BooleanExpression");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int) GrammarConstants.COMP_EXPRESSION, 1, 1);
            alt.AddProduction((int) GrammarConstants.BOOLEAN_EXPRESSION_TAIL, 0, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) GrammarConstants.BOOLEAN_EXPRESSION_TAIL,
                                            "BooleanExpressionTail");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) GrammarConstants.AND, 1, 1);
            alt.AddProduction((int) GrammarConstants.BOOLEAN_EXPRESSION, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) GrammarConstants.OR, 1, 1);
            alt.AddProduction((int) GrammarConstants.BOOLEAN_EXPRESSION, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) GrammarConstants.COMP_EXPRESSION,
                                            "CompExpression");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int) GrammarConstants.ARITH_EXPRESSION, 1, 1);
            alt.AddProduction((int) GrammarConstants.COMP_EXPRESSION_TAIL, 0, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) GrammarConstants.COMP_EXPRESSION_TAIL,
                                            "CompExpressionTail");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) GrammarConstants.GTE, 1, 1);
            alt.AddProduction((int) GrammarConstants.COMP_EXPRESSION, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) GrammarConstants.LTE, 1, 1);
            alt.AddProduction((int) GrammarConstants.COMP_EXPRESSION, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) GrammarConstants.EQ, 1, 1);
            alt.AddProduction((int) GrammarConstants.COMP_EXPRESSION, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) GrammarConstants.GT, 1, 1);
            alt.AddProduction((int) GrammarConstants.COMP_EXPRESSION, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) GrammarConstants.LT, 1, 1);
            alt.AddProduction((int) GrammarConstants.COMP_EXPRESSION, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) GrammarConstants.ARITH_EXPRESSION,
                                            "ArithExpression");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int) GrammarConstants.TERM, 1, 1);
            alt.AddProduction((int) GrammarConstants.ARITH_EXPRESSION_TAIL, 0, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) GrammarConstants.ARITH_EXPRESSION_TAIL,
                                            "ArithExpressionTail");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) GrammarConstants.PLUS, 1, 1);
            alt.AddProduction((int) GrammarConstants.EXPRESSION, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) GrammarConstants.MINUS, 1, 1);
            alt.AddProduction((int) GrammarConstants.EXPRESSION, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) GrammarConstants.TERM,
                                            "Term");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int) GrammarConstants.FACTOR, 1, 1);
            alt.AddProduction((int) GrammarConstants.TERM_TAIL, 0, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) GrammarConstants.TERM_TAIL,
                                            "TermTail");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) GrammarConstants.STAR, 0, 1);
            alt.AddProduction((int) GrammarConstants.TERM, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) GrammarConstants.SLASH, 1, 1);
            alt.AddProduction((int) GrammarConstants.TERM, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) GrammarConstants.FACTOR,
                                            "Factor");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int) GrammarConstants.ATOM, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) GrammarConstants.LPAR, 1, 1);
            alt.AddProduction((int) GrammarConstants.EXPRESSION, 1, 1);
            alt.AddToken((int) GrammarConstants.RPAR, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) GrammarConstants.MINUS, 1, 1);
            alt.AddProduction((int) GrammarConstants.FACTOR, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int) GrammarConstants.FUNCTION_EXPRESSION, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) GrammarConstants.ATOM,
                                            "Atom");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) GrammarConstants.NUMBER, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) GrammarConstants.IDENTIFIER, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) SynteticPatterns.SUBPRODUCTION_1,
                                            "Subproduction1");
            pattern.Synthetic = true;
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) GrammarConstants.COMMA, 1, 1);
            alt.AddProduction((int) GrammarConstants.EXPRESSION, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);
        }
    }
}
