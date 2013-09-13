/*
 * GrammarTokenizer.cs
 *
 * THIS FILE HAS BEEN GENERATED AUTOMATICALLY. DO NOT EDIT!
 */

using System.IO;

using PerCederberg.Grammatica.Runtime;

namespace Rw.Parsing.Generated {

    /**
     * <remarks>A character stream tokenizer.</remarks>
     */
    internal class GrammarTokenizer : Tokenizer {

        /**
         * <summary>Creates a new tokenizer for the specified input
         * stream.</summary>
         *
         * <param name='input'>the input stream to read</param>
         *
         * <exception cref='ParserCreationException'>if the tokenizer
         * couldn't be initialized correctly</exception>
         */
        public GrammarTokenizer(TextReader input)
            : base(input, false) {

            CreatePatterns();
        }

        /**
         * <summary>Initializes the tokenizer by creating all the token
         * patterns.</summary>
         *
         * <exception cref='ParserCreationException'>if the tokenizer
         * couldn't be initialized correctly</exception>
         */
        private void CreatePatterns() {
            TokenPattern  pattern;

            pattern = new TokenPattern((int) GrammarConstants.PLUS,
                                       "PLUS",
                                       TokenPattern.PatternType.STRING,
                                       "+");
            AddPattern(pattern);

            pattern = new TokenPattern((int) GrammarConstants.MINUS,
                                       "MINUS",
                                       TokenPattern.PatternType.STRING,
                                       "-");
            AddPattern(pattern);

            pattern = new TokenPattern((int) GrammarConstants.STAR,
                                       "STAR",
                                       TokenPattern.PatternType.STRING,
                                       "*");
            AddPattern(pattern);

            pattern = new TokenPattern((int) GrammarConstants.SLASH,
                                       "SLASH",
                                       TokenPattern.PatternType.STRING,
                                       "/");
            AddPattern(pattern);

            pattern = new TokenPattern((int) GrammarConstants.CARET,
                                       "CARET",
                                       TokenPattern.PatternType.STRING,
                                       "^");
            AddPattern(pattern);

            pattern = new TokenPattern((int) GrammarConstants.BACK_SLASH,
                                       "BACK_SLASH",
                                       TokenPattern.PatternType.STRING,
                                       "\\");
            AddPattern(pattern);

            pattern = new TokenPattern((int) GrammarConstants.LPAR,
                                       "LPAR",
                                       TokenPattern.PatternType.STRING,
                                       "(");
            AddPattern(pattern);

            pattern = new TokenPattern((int) GrammarConstants.RPAR,
                                       "RPAR",
                                       TokenPattern.PatternType.STRING,
                                       ")");
            AddPattern(pattern);

            pattern = new TokenPattern((int) GrammarConstants.COMMA,
                                       "COMMA",
                                       TokenPattern.PatternType.STRING,
                                       ",");
            AddPattern(pattern);

            pattern = new TokenPattern((int) GrammarConstants.SEMICOLON,
                                       "SEMICOLON",
                                       TokenPattern.PatternType.STRING,
                                       ";");
            AddPattern(pattern);

            pattern = new TokenPattern((int) GrammarConstants.COLON,
                                       "COLON",
                                       TokenPattern.PatternType.STRING,
                                       ":");
            AddPattern(pattern);

            pattern = new TokenPattern((int) GrammarConstants.DEFEQ,
                                       "DEFEQ",
                                       TokenPattern.PatternType.STRING,
                                       ":=");
            AddPattern(pattern);

            pattern = new TokenPattern((int) GrammarConstants.YIELD,
                                       "YIELD",
                                       TokenPattern.PatternType.STRING,
                                       "->");
            AddPattern(pattern);

            pattern = new TokenPattern((int) GrammarConstants.LET,
                                       "LET",
                                       TokenPattern.PatternType.STRING,
                                       "let ");
            AddPattern(pattern);

            pattern = new TokenPattern((int) GrammarConstants.IN,
                                       "IN",
                                       TokenPattern.PatternType.STRING,
                                       "in ");
            AddPattern(pattern);

            pattern = new TokenPattern((int) GrammarConstants.DEF,
                                       "DEF",
                                       TokenPattern.PatternType.STRING,
                                       "def ");
            AddPattern(pattern);

            pattern = new TokenPattern((int) GrammarConstants.WHERE,
                                       "WHERE",
                                       TokenPattern.PatternType.STRING,
                                       "where ");
            AddPattern(pattern);

            pattern = new TokenPattern((int) GrammarConstants.IF,
                                       "IF",
                                       TokenPattern.PatternType.STRING,
                                       "if ");
            AddPattern(pattern);

            pattern = new TokenPattern((int) GrammarConstants.THEN,
                                       "THEN",
                                       TokenPattern.PatternType.STRING,
                                       "then ");
            AddPattern(pattern);

            pattern = new TokenPattern((int) GrammarConstants.ELSE,
                                       "ELSE",
                                       TokenPattern.PatternType.STRING,
                                       "else ");
            AddPattern(pattern);

            pattern = new TokenPattern((int) GrammarConstants.AND,
                                       "AND",
                                       TokenPattern.PatternType.STRING,
                                       "and ");
            AddPattern(pattern);

            pattern = new TokenPattern((int) GrammarConstants.OR,
                                       "OR",
                                       TokenPattern.PatternType.STRING,
                                       "or ");
            AddPattern(pattern);

            pattern = new TokenPattern((int) GrammarConstants.GTE,
                                       "GTE",
                                       TokenPattern.PatternType.STRING,
                                       ">=");
            AddPattern(pattern);

            pattern = new TokenPattern((int) GrammarConstants.LTE,
                                       "LTE",
                                       TokenPattern.PatternType.STRING,
                                       "<=");
            AddPattern(pattern);

            pattern = new TokenPattern((int) GrammarConstants.EQ,
                                       "EQ",
                                       TokenPattern.PatternType.STRING,
                                       "=");
            AddPattern(pattern);

            pattern = new TokenPattern((int) GrammarConstants.GT,
                                       "GT",
                                       TokenPattern.PatternType.STRING,
                                       ">");
            AddPattern(pattern);

            pattern = new TokenPattern((int) GrammarConstants.LT,
                                       "LT",
                                       TokenPattern.PatternType.STRING,
                                       "<");
            AddPattern(pattern);

            pattern = new TokenPattern((int) GrammarConstants.NUMBER,
                                       "NUMBER",
                                       TokenPattern.PatternType.REGEXP,
                                       "-?(\\d+)");
            AddPattern(pattern);

            pattern = new TokenPattern((int) GrammarConstants.DECIMAL,
                                       "DECIMAL",
                                       TokenPattern.PatternType.REGEXP,
                                       "-?(\\d*)\\.\\d+");
            AddPattern(pattern);

            pattern = new TokenPattern((int) GrammarConstants.IDENTIFIER,
                                       "IDENTIFIER",
                                       TokenPattern.PatternType.REGEXP,
                                       "[a-z_][a-zA-Z_?0-9]*");
            AddPattern(pattern);

            pattern = new TokenPattern((int) GrammarConstants.WHITESPACE,
                                       "WHITESPACE",
                                       TokenPattern.PatternType.REGEXP,
                                       "[ \\t\\n\\r]+");
            pattern.Ignore = true;
            AddPattern(pattern);
        }
    }
}
