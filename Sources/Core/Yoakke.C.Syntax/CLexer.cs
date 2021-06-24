// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.Collections;
using Yoakke.Lexer;
using Yoakke.Text;

namespace Yoakke.C.Syntax
{
    /// <summary>
    /// A lexer that lexes C source tokens, including preprocessor directives.
    /// </summary>
    public class CLexer : LexerBase<CToken>
    {
        /// <summary>
        /// The logical <see cref="Position"/> in the source.
        /// This takes line continuations as being in the same line as the previous one for example.
        /// </summary>
        public Position LogicalPosition { get; private set; }

        /// <summary>
        /// True, if line continuations should be enabled with '\'.
        /// </summary>
        public bool AllowLineContinuations { get; set; } = true;

        /// <summary>
        /// True, if digraphs should be enabled.
        /// </summary>
        public bool AllowDigraphs { get; set; } = true;

        /// <summary>
        /// True, if trigraphs should be enabled.
        /// </summary>
        public bool AllowTrigraphs { get; set; } = true;

        public CLexer(TextReader reader)
            : base(reader)
        {
        }

        public CLexer(string source)
            : base(source)
        {
        }

        public override CToken Next()
        {
            var offset = 0;

            CToken Make(CTokenType type, string text) => this.TakeToken(type, offset, text);

        begin:
            // Since we can jump back here, we need to reset
            this.Skip(offset);
            offset = 0;

            // EOF
            if (!this.TryParseEscaped(out var peek, ref offset)) return this.TakeToken(CTokenType.End, 0, string.Empty);

            // Check for newline
            if (peek == '\r' || peek == '\n')
            {
                // If it was a '\r', consume '\n' because it might be Windows-style
                if (peek == '\r') this.MatchesEscaped('\n', ref offset);
                // Was a newline anyway
                this.LogicalPosition = this.LogicalPosition.Newline();
                goto begin;
            }

            // Whitespace is horizontal skip, as well as the NUL character
            if (char.IsWhiteSpace(peek) || peek == '\0')
            {
                this.LogicalPosition = this.LogicalPosition.Advance();
                goto begin;
            }

            // Control character is just skip
            if (char.IsControl(peek)) goto begin;

            // Check for comments
            if (peek == '/' && this.MatchesEscaped('/', ref offset))
            {
                // Line-comment, go until the end of line
                char lastChar;
                while (this.TryParseEscaped(out lastChar, ref offset) && !IsNewline(lastChar))
                {
                    // Pass
                }
                // End of line reached
                if (lastChar == '\r') this.MatchesEscaped('\n', ref offset);
                // A line comment becomes a space
                this.LogicalPosition = this.LogicalPosition.Advance();
                goto begin;
            }
            if (peek == '/' && this.MatchesEscaped('*', ref offset))
            {
                // Multi-line comment
                while (true)
                {
                    if (!this.TryParseEscaped(out var ch, ref offset)) break;
                    if (ch == '*' && this.MatchesEscaped('/', ref offset)) break;
                }
                // A multi-line comment becomes a space too
                this.LogicalPosition = this.LogicalPosition.Advance();
                goto begin;
            }

            // Try to decide on the current character
            switch (peek)
            {
            case ',': return Make(CTokenType.Comma, ",");
            case ';': return Make(CTokenType.Semicolon, ";");
            case '?': return Make(CTokenType.QuestionMark, "?");
            case '~': return Make(CTokenType.BitNot, "~");
            case '(': return Make(CTokenType.OpenParen, "(");
            case ')': return Make(CTokenType.CloseParen, ")");
            case '{': return Make(CTokenType.OpenBrace, "{");
            case '}': return Make(CTokenType.CloseBrace, "}");
            case '[': return Make(CTokenType.OpenBracket, "[");
            case ']': return Make(CTokenType.CloseBracket, "]");

            case ':': return this.MatchesEscaped('>', ref offset)
                    ? Make(CTokenType.CloseBracket, ":>")
                    : Make(CTokenType.Colon, ":");
            case '*': return this.MatchesEscaped('=', ref offset)
                    ? Make(CTokenType.MultiplyAssign, "*=")
                    : Make(CTokenType.Multiply, "*");
            case '/': return this.MatchesEscaped('=', ref offset)
                    ? Make(CTokenType.DivideAssign, "/=")
                    : Make(CTokenType.Divide, "/");
            case '!': return this.MatchesEscaped('=', ref offset)
                    ? Make(CTokenType.NotEqual, "!=")
                    : Make(CTokenType.LogicalNot, "!");
            case '^': return this.MatchesEscaped('=', ref offset)
                    ? Make(CTokenType.BitXorAssign, "^=")
                    : Make(CTokenType.BitXor, "^");
            case '=': return this.MatchesEscaped('=', ref offset)
                    ? Make(CTokenType.Equal, "==")
                    : Make(CTokenType.Assign, "=");
            case '#': return this.MatchesEscaped('#', ref offset)
                    ? Make(CTokenType.HashHash, "##")
                    : Make(CTokenType.Hash, "#");

            case '+':
                if (this.MatchesEscaped('+', ref offset)) return Make(CTokenType.Increment, "++");
                if (this.MatchesEscaped('=', ref offset)) return Make(CTokenType.AddAssign, "+=");
                return Make(CTokenType.Add, "=");
            case '|':
                if (this.MatchesEscaped('|', ref offset)) return Make(CTokenType.LogicalOr, "||");
                if (this.MatchesEscaped('=', ref offset)) return Make(CTokenType.BitOrAssign, "|=");
                return Make(CTokenType.BitOr, "|");
            case '&':
                if (this.MatchesEscaped('&', ref offset)) return Make(CTokenType.LogicalAnd, "&&");
                if (this.MatchesEscaped('=', ref offset)) return Make(CTokenType.BitAndAssign, "&=");
                return Make(CTokenType.BitAnd, "&");

            case '-':
                if (this.MatchesEscaped('-', ref offset)) return Make(CTokenType.Decrement, "--");
                if (this.MatchesEscaped('>', ref offset)) return Make(CTokenType.Arrow, "->");
                if (this.MatchesEscaped('=', ref offset)) return Make(CTokenType.SubtractAssign, "-=");
                return Make(CTokenType.Subtract, "-");
            case '>':
                if (this.MatchesEscaped(">=", ref offset)) return Make(CTokenType.ShiftRightAssign, ">>=");
                if (this.MatchesEscaped('>', ref offset)) return Make(CTokenType.ShiftRight, ">>");
                if (this.MatchesEscaped('=', ref offset)) return Make(CTokenType.GreaterEqual, ">=");
                return Make(CTokenType.Greater, ">");

            case '<':
                if (this.MatchesEscaped("<=", ref offset)) return Make(CTokenType.ShiftLeftAssign, "<<=");
                if (this.MatchesEscaped('<', ref offset)) return Make(CTokenType.ShiftLeft, "<<");
                if (this.MatchesEscaped('=', ref offset)) return Make(CTokenType.LessEqual, "<=");
                if (this.MatchesEscaped(':', ref offset)) return Make(CTokenType.OpenBracket, "<:");
                if (this.MatchesEscaped('%', ref offset)) return Make(CTokenType.OpenBrace, "<%");
                return Make(CTokenType.Less, "<");

            case '%':
                if (this.MatchesEscaped(":%:", ref offset)) return Make(CTokenType.HashHash, "%:%:");
                if (this.MatchesEscaped(':', ref offset)) return Make(CTokenType.Hash, "%:");
                if (this.MatchesEscaped('>', ref offset)) return Make(CTokenType.CloseBrace, "%>");
                if (this.MatchesEscaped('=', ref offset)) return Make(CTokenType.ModuloAssign, "%=");
                return Make(CTokenType.Modulo, "%");

            case '.':
                if (this.MatchesEscaped("..", ref offset)) return Make(CTokenType.Ellipsis, "...");
                // If it's a digit, it's a float, don't handle it here
                if (!char.IsDigit(this.ParseEscaped(offset, out var _))) return Make(CTokenType.Dot, ".");
                break;
            }

            // Unknown
            return this.TakeToken(CTokenType.Unknown, offset, peek.ToString());
        }

        private bool TryParseExponent(int offset, out int nextOffset)
        {
            var initial = offset;
            if (IsE(this.ParseEscaped(ref offset)))
            {
                if (IsSign(this.ParseEscaped(offset, out var offset2))) offset = offset2;
                var anyDigits = false;
                for (; char.IsDigit(this.ParseEscaped(ref offset)); anyDigits = true) ;
            }
            nextOffset = initial;
            return false;
        }

        /// <summary>
        /// Wrapper for <see cref="MatchesEscaped(string, int, out int)"/>.
        /// </summary>
        private bool MatchesEscaped(string text, ref int offset) =>
            this.MatchesEscaped(text, offset, out offset);

        /// <summary>
        /// Tries to match an escaped string from the input.
        /// </summary>
        /// <param name="text">The text to match.</param>
        /// <param name="offset">The offset to start the match from.</param>
        /// <param name="nextOffset">The offset gets written here after the match.</param>
        /// <returns>True, if the text matches.</returns>
        private bool MatchesEscaped(string text, int offset, out int nextOffset)
        {
            var initial = offset;
            for (int i = 0; i < text.Length; ++i)
            {
                if (!this.MatchesEscaped(text[i], offset, out nextOffset))
                {
                    nextOffset = initial;
                    return false;
                }
                offset = nextOffset;
            }
            nextOffset = offset;
            return true;
        }

        /// <summary>
        /// Wrapper for <see cref="MatchesEscaped(char, int, out int)"/>.
        /// </summary>
        private bool MatchesEscaped(char ch, ref int offset) =>
            this.MatchesEscaped(ch, offset, out offset);

        /// <summary>
        /// Tries to match an escaped character in the input.
        /// </summary>
        /// <param name="ch">The character to match.</param>
        /// <param name="offset">The offset to match at.</param>
        /// <param name="nextOffset">The offset gets written here after the match.</param>
        /// <returns>True, if the character matches.</returns>
        private bool MatchesEscaped(char ch, int offset, out int nextOffset)
        {
            if (this.TryParseEscaped(out var got, offset, out var offset2) && got == ch)
            {
                nextOffset = offset2;
                return true;
            }
            else
            {
                nextOffset = offset;
                return false;
            }
        }

        /// <summary>
        /// Wrapper for <see cref="ParseEscaped(int, out int, char)"/>.
        /// </summary>
        private char ParseEscaped(ref int offset, char @default = '\0') =>
            this.ParseEscaped(offset, out offset, @default);

        /// <summary>
        /// Parses an escaped character from the input.
        /// </summary>
        /// <param name="offset">The offset to start the parse from.</param>
        /// <param name="nextOffset">The offset gets written here after the parse.</param>
        /// <param name="default">The default character that gets returned if the end of input is reached.</param>
        /// <returns>The parsed character, or <paramref name="default"/>, if the end of input is reached.</returns>
        private char ParseEscaped(int offset, out int nextOffset, char @default = '\0') =>
            this.TryParseEscaped(out var ch, offset, out nextOffset) ? ch : @default;

        /// <summary>
        /// Wrapper for <see cref="TryParseEscaped(out char, int, out int)"/>.
        /// </summary>
        private bool TryParseEscaped(out char result, ref int offset) =>
            this.TryParseEscaped(out result, offset, out offset);

        /// <summary>
        /// Tries to parse the next escaped character from the input.
        /// Escaped means line-continuated - if enabled - and trighraph-converted - also if enabled.
        /// </summary>
        /// <param name="result">The resulting escaped character is written here.</param>
        /// <param name="offset">The offset to start the parse from.</param>
        /// <param name="nextOffset">The offset that ends the character is written here.</param>
        /// <returns>True, if there was a character to parse, false otherwise.</returns>
        private bool TryParseEscaped(out char result, int offset, out int nextOffset)
        {
        begin:
            // If there is no character, just return immediately
            if (!this.TryPeek(out var ch, offset))
            {
                result = default;
                nextOffset = offset;
                return false;
            }

            if (this.AllowLineContinuations)
            {
                // Line-continuations are enabled, if there is a '\', means a potential line-continuation
                var backslashLength = 0;
                if (ch == '\\') backslashLength = 1;
                else if (this.AllowTrigraphs && this.Matches("??/")) backslashLength = 3;

                // A length > 0 means that there was a '\' or equivalent trigraph
                if (backslashLength > 0)
                {
                    var length = backslashLength;
                    // Consume whitespace, we allow trailing spaces before the newline
                    for (; IsSpace(this.Peek(length)); ++length)
                    {
                        // Pass
                    }
                    // Now we expect a newline
                    var newline = 0;
                    if (this.Matches("\r\n", length)) newline = 2;
                    else if (this.Matches('\r', length) || this.Matches('\n', length)) newline = 1;
                    // If we got a newline, it's a line-continuation
                    if (newline > 0)
                    {
                        // It's a line-continuation, retry after consume
                        offset = length + newline;
                        goto begin;
                    }
                    // Otherwise we just eat the backslash
                    result = '\\';
                    nextOffset = backslashLength;
                    return true;
                }
            }

            // It could be a trigraph
            if (this.AllowTrigraphs && this.Matches("??", offset))
            {
                char? trigraph = this.TryPeek(out var ch2, 2) ? ch2 switch
                {
                    '=' => '#',
                    '/' => '\\',
                    '\'' => '^',
                    '(' => '[',
                    ')' => ']',
                    '!' => '|',
                    '<' => '{',
                    '>' => '}',
                    '-' => '~',
                    _ => null,
                } : null;
                if (trigraph is not null)
                {
                    // It was a trigraph
                    result = trigraph.Value;
                    nextOffset = offset + 3;
                    return true;
                }
            }

            // Out of ideas, just return the character raw
            result = ch;
            nextOffset = offset + 1;
            return true;
        }

        /// <summary>
        /// Constructs a new <see cref="CToken"/>.
        /// </summary>
        /// <param name="type">The <see cref="CTokenType"/>.</param>
        /// <param name="length">The physical length to consume.</param>
        /// <param name="logicalText">The logical (escaped) text of the token.</param>
        /// <returns>The constructed <see cref="CToken"/>.</returns>
        private CToken TakeToken(CTokenType type, int length, string logicalText)
        {
            // Construct the logical range
            var startPosition = this.LogicalPosition;
            this.LogicalPosition = this.LogicalPosition.Advance(logicalText.Length);
            var logicalRange = new Text.Range(startPosition, this.LogicalPosition);
            // Construct the token
            return this.TakeToken(length, (range, text) => new(range, text, logicalRange, logicalText, type));
        }

        private static bool IsSpace(char ch) => ch == ' ' || ch == '\0';

        private static bool IsNewline(char ch) => ch == '\n' || ch == '\r';

        private static bool IsIdent(char ch) => char.IsLetterOrDigit(ch) || ch == '_';

        private static bool IsHex(char ch) => "0123456789abcdefABCDEF".Contains(ch);

        private static bool IsE(char ch) => "eE".Contains(ch);
        private static bool IsSign(char ch) => "+-".Contains(ch);

        private static bool IsFloatSuffix(char ch) => "fFlL".Contains(ch);

        private static bool IsIntSuffix(char ch) => "uUlL".Contains(ch);
    }
}
