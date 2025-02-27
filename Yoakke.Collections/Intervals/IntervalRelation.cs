﻿using System;
using System.Collections.Generic;
using Yoakke.Collections.Compatibility;

namespace Yoakke.Collections.Intervals
{
    /// <summary>
    /// Base class for describing the relation of two intervals.
    /// </summary>
    /// <typeparam name="T">The type of the interval values.</typeparam>
    public abstract class IntervalRelation<T> : IEquatable<IntervalRelation<T>>
    {
        public abstract bool Equals(IntervalRelation<T> other, IComparer<T> comparer);
        public abstract override int GetHashCode();

        public override bool Equals(object obj) => obj is IntervalRelation<T> ir && Equals(ir);
        public bool Equals(IntervalRelation<T> other) => Equals(other, Comparer<T>.Default);

        public static bool operator ==(IntervalRelation<T> a, IntervalRelation<T> b) => a.Equals(b);
        public static bool operator !=(IntervalRelation<T> a, IntervalRelation<T> b) => !a.Equals(b);

        /// <summary>
        /// Represents that the two intervals are completely disjunct.
        /// </summary>
        public class Disjunct : IntervalRelation<T>
        {
            /// <summary>
            /// The first interval of the disjunct pair.
            /// </summary>
            public readonly Interval<T> First;
            /// <summary>
            /// The second interval of the disjunct pair.
            /// </summary>
            public readonly Interval<T> Second;

            public Disjunct(Interval<T> first, Interval<T> second)
            {
                First = first;
                Second = second;
            }

            public override bool Equals(IntervalRelation<T> other, IComparer<T> comparer) =>
                other is Disjunct d && First.Equals(d.First, comparer) && Second.Equals(d.Second, comparer);
            public override int GetHashCode() => HashCode.Combine(First, Second);
        }

        /// <summary>
        /// Represents that the two intervals are touching.
        /// </summary>
        public class Touching : IntervalRelation<T>
        {
            /// <summary>
            /// The first interval of the touching pair.
            /// </summary>
            public readonly Interval<T> First;
            /// <summary>
            /// The second interval of the touching pair.
            /// </summary>
            public readonly Interval<T> Second;

            public Touching(Interval<T> first, Interval<T> second)
            {
                First = first;
                Second = second;
            }

            public override bool Equals(IntervalRelation<T> other, IComparer<T> comparer) =>
                other is Touching t && First.Equals(t.First, comparer) && Second.Equals(t.Second, comparer);
            public override int GetHashCode() => HashCode.Combine(First, Second);
        }

        /// <summary>
        /// Represents that the two intervals are overlapping.
        /// </summary>
        public class Overlapping : IntervalRelation<T>
        {
            /// <summary>
            /// The first disjunct part of the intervals (before the overlap).
            /// </summary>
            public readonly Interval<T> FirstDisjunct;
            /// <summary>
            /// The overlapping portion of the intervals.
            /// </summary>
            public readonly Interval<T> Overlap;
            /// <summary>
            /// The second disjunct part of the intervals (after the overlap).
            /// </summary>
            public readonly Interval<T> SecondDisjunct;

            public Overlapping(Interval<T> firstDisjunct, Interval<T> overlap, Interval<T> secondDisjunct)
            {
                FirstDisjunct = firstDisjunct;
                Overlap = overlap;
                SecondDisjunct = secondDisjunct;
            }

            public override bool Equals(IntervalRelation<T> other, IComparer<T> comparer) =>
                   other is Overlapping o 
                && FirstDisjunct.Equals(o.FirstDisjunct, comparer) 
                && Overlap.Equals(o.Overlap, comparer) 
                && SecondDisjunct.Equals(o.SecondDisjunct, comparer);
            public override int GetHashCode() => HashCode.Combine(FirstDisjunct, Overlap, SecondDisjunct);
        }

        /// <summary>
        /// Represents that one interval completely contains the other.
        /// </summary>
        public class Containing : IntervalRelation<T>
        {
            /// <summary>
            /// The first disjunct part of the intervals (before the containment).
            /// </summary>
            public readonly Interval<T> FirstDisjunct;
            /// <summary>
            /// The contained portion of the intervals.
            /// </summary>
            public readonly Interval<T> Contained;
            /// <summary>
            /// The second disjunct part of the intervals (after the containment).
            /// </summary>
            public readonly Interval<T> SecondDisjunct;

            public Containing(Interval<T> firstDisjunct, Interval<T> contained, Interval<T> secondDisjunct)
            {
                FirstDisjunct = firstDisjunct;
                Contained = contained;
                SecondDisjunct = secondDisjunct;
            }

            public override bool Equals(IntervalRelation<T> other, IComparer<T> comparer) =>
                   other is Containing c
                && FirstDisjunct.Equals(c.FirstDisjunct, comparer) 
                && Contained.Equals(c.Contained, comparer) 
                && SecondDisjunct.Equals(c.SecondDisjunct, comparer);
            public override int GetHashCode() => HashCode.Combine(FirstDisjunct, Contained, SecondDisjunct);
        }

        /// <summary>
        /// Represents that one interval starts with the other.
        /// </summary>
        public class Starting : IntervalRelation<T>
        {
            /// <summary>
            /// The overlapping portion of the intervals (start).
            /// </summary>
            public readonly Interval<T> Overlap;
            /// <summary>
            /// The disjunct part of the intervals (after the overlap).
            /// </summary>
            new public readonly Interval<T> Disjunct;

            public Starting(Interval<T> overlap, Interval<T> disjunct)
            {
                Overlap = overlap;
                Disjunct = disjunct;
            }

            public override bool Equals(IntervalRelation<T> other, IComparer<T> comparer) =>
                other is Starting s && Overlap.Equals(s.Overlap, comparer) && Disjunct.Equals(s.Disjunct, comparer);
            public override int GetHashCode() => HashCode.Combine(Overlap, Disjunct);
        }

        /// <summary>
        /// Represents that one interval finishes with the other.
        /// </summary>
        public class Finishing : IntervalRelation<T>
        {
            /// <summary>
            /// The disjunct part of the intervals (after the before).
            /// </summary>
            new public readonly Interval<T> Disjunct;
            /// <summary>
            /// The overlapping portion of the intervals (end).
            /// </summary>
            public readonly Interval<T> Overlap;

            public Finishing(Interval<T> disjunct, Interval<T> overlap)
            {
                Disjunct = disjunct;
                Overlap = overlap;
            }

            public override bool Equals(IntervalRelation<T> other, IComparer<T> comparer) =>
                other is Finishing f && Overlap.Equals(f.Overlap, comparer) && Disjunct.Equals(f.Disjunct, comparer);
            public override int GetHashCode() => HashCode.Combine(Disjunct, Overlap);
        }

        /// <summary>
        /// Represents that the two intervals are completely equal.
        /// </summary>
        public class Equal : IntervalRelation<T>
        {
            /// <summary>
            /// The equal intervals.
            /// </summary>
            public readonly Interval<T> Interval;

            public Equal(Interval<T> interval)
            {
                Interval = interval;
            }

            public override bool Equals(IntervalRelation<T> other, IComparer<T> comparer) =>
                other is Equal e && Interval.Equals(e.Interval, comparer);
            public override int GetHashCode() => Interval.GetHashCode();
        }
    }
}
