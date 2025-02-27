﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Reporting
{
    /// <summary>
    /// Represents a severity level for the library.
    /// Even though the severity instances have a name, their identities are defined by their priotiries only.
    /// </summary>
    public readonly struct Severity : IEquatable<Severity>, IComparable<Severity>
    {
        public static readonly Severity Note = new Severity("note", 0);
        public static readonly Severity Help = new Severity("help", 1);
        public static readonly Severity Warning = new Severity("warning", 2);
        public static readonly Severity Error = new Severity("error", 3);
        public static readonly Severity InternalError = new Severity("internal error", 4);

        /// <summary>
        /// The name of the severity level.
        /// </summary>
        public readonly string Name;
        /// <summary>
        /// The priority of the severity level.
        /// </summary>
        public readonly int Priority;

        /// <summary>
        /// Initializes a new <see cref="Severity"/>.
        /// </summary>
        /// <param name="name">The name of the severity level.</param>
        /// <param name="priority">The priority of the severity level.</param>
        public Severity(string name, int priority)
        {
            Name = name;
            Priority = priority;
        }

        public override bool Equals(object? obj) => obj is Severity s && Equals(s);
        public bool Equals(Severity other) => Priority == other.Priority;
        public override int GetHashCode() => Priority.GetHashCode();
        public int CompareTo(Severity other) => Priority - other.Priority;

        public static bool operator ==(Severity left, Severity right) => left.Equals(right);
        public static bool operator !=(Severity left, Severity right) => !left.Equals(right);

        public static bool operator <(Severity left, Severity right) => left.CompareTo(right) < 0;
        public static bool operator <=(Severity left, Severity right) => left.CompareTo(right) <= 0;
        public static bool operator >(Severity left, Severity right) => left.CompareTo(right) > 0;
        public static bool operator >=(Severity left, Severity right) => left.CompareTo(right) >= 0;
    }
}
