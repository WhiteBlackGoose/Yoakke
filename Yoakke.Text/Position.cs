﻿using System;

namespace Yoakke.Text
{
    /// <summary>
    /// Represents 2D position inside some text.
    /// </summary>
    public readonly struct Position : IEquatable<Position>, IComparable<Position>
    {
        /// <summary>
        /// The 0-based line index.
        /// </summary>
        public readonly int Line;
        /// <summary>
        /// The 0-based column index.
        /// </summary>
        public readonly int Column;

        /// <summary>
        /// Initializes a new <see cref="Position"/>.
        /// </summary>
        /// <param name="line">The 0-based line index.</param>
        /// <param name="column">The 0-based column index.</param>
        public Position(int line, int column)
        {
            Line = line;
            Column = column;
        }

        public override bool Equals(object? obj) => obj is Position position && Equals(position);
        public override int GetHashCode() => HashCode.Combine(Line, Column);
        public bool Equals(Position other) => CompareTo(other) == 0;

        public int CompareTo(Position other)
        {
            var l = Line.CompareTo(other.Line);
            return l == 0 ? Column.CompareTo(other.Column) : l;
        }

        public static bool operator ==(Position p1, Position p2) => p1.CompareTo(p2) == 0;
        public static bool operator !=(Position p1, Position p2) => p1.CompareTo(p2) != 0;
        public static bool operator <(Position p1, Position p2) => p1.CompareTo(p2) < 0;
        public static bool operator >(Position p1, Position p2) => p1.CompareTo(p2) > 0;
        public static bool operator <=(Position p1, Position p2) => p1.CompareTo(p2) <= 0;
        public static bool operator >=(Position p1, Position p2) => p1.CompareTo(p2) >= 0;

        public override string ToString() => $"line {Line + 1}, column {Column + 1}";

        /// <summary>
        /// Creates a <see cref="Position"/> that's advanced in the current line by the given amount.
        /// </summary>
        /// <param name="amount">The amount to advance in the current line.</param>
        /// <returns>The <see cref="Position"/> in the same line, advanced by columns.</returns>
        public Position Advance(int amount = 1) => new Position(line: Line, column: Column + amount);

        /// <summary>
        /// Creates a <see cref="Position"/> that points to the first character of the next line.
        /// </summary>
        /// <returns>A <see cref="Position"/> in the next line's first character.</returns>
        public Position Newline() => new Position(line: Line + 1, column: 0);
    }
}
