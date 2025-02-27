﻿using System;
using Range = Yoakke.Text.Range;

namespace Yoakke.Lexer
{
    /// <summary>
    /// Represents an atom in a language grammar as the lowest level element (atom/terminal) of parsing.
    /// This is a tagged implementation of <see cref="IToken"/>.
    /// </summary>
    /// <typeparam name="TKind">The kind type this <see cref="Token{TKind}"/> uses. Usually an enumeration type.</typeparam>
    public class Token<TKind> : IToken, IEquatable<Token<TKind>> where TKind : notnull
    {
        public Range Range { get; }
        public string Text { get; }

        /// <summary>
        /// The kind tag of this <see cref="Token{TKind}"/>.
        /// </summary>
        public readonly TKind Kind;

        /// <summary>
        /// Initializes a new <see cref="Token{TKind}"/>.
        /// </summary>
        /// <param name="range">The <see cref="Text.Range"/> of the <see cref="Token{TKind}"/> in the source.</param>
        /// <param name="text">The text the <see cref="Token{TKind}"/> was parsed from.</param>
        /// <param name="kind">The <see cref="TKind"/> of the <see cref="Token{TKind}"/>.</param>
        public Token(Range range, string text, TKind kind)
        {
            Range = range;
            Text = text;
            Kind = kind;
        }

        public override bool Equals(object? obj) => Equals(obj as Token<TKind>);
        public bool Equals(IToken? other) => Equals(other as Token<TKind>);
        public bool Equals(Token<TKind>? other) =>
               other is not null
            && Range == other.Range
            && Text == other.Text
            && Kind.Equals(other.Kind);

        public override int GetHashCode() => HashCode.Combine(Range, Text);
    }
}
