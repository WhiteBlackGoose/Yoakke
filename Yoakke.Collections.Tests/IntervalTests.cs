﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.Collections.Intervals;

namespace Yoakke.Collections.Tests
{
    [TestClass]
    public class IntervalTests
    {
        private static Interval<int> Iv(string s)
        {
            if (s == "..") return Interval<int>.Full();
            if (s.StartsWith("..=")) return new Interval<int>(LowerBound<int>.Unbounded(), UpperBound<int>.Inclusive(int.Parse(s.Substring(3))));
            if (s.StartsWith("..")) return new Interval<int>(LowerBound<int>.Unbounded(), UpperBound<int>.Exclusive(int.Parse(s.Substring(3))));
            if (s.EndsWith("..")) return new Interval<int>(LowerBound<int>.Inclusive(int.Parse(s.Substring(0, s.Length - 2))), UpperBound<int>.Unbounded());
            if (s.Contains("..="))
            {
                var parts = s.Split("..=");
                return new Interval<int>(LowerBound<int>.Inclusive(int.Parse(parts[0])), UpperBound<int>.Inclusive(int.Parse(parts[1])));
            }
            if (s.Contains(".."))
            {
                var parts = s.Split("..");
                return new Interval<int>(LowerBound<int>.Inclusive(int.Parse(parts[0])), UpperBound<int>.Exclusive(int.Parse(parts[1])));
            }
            throw new NotImplementedException();
        }

        private static IntervalRelation<int> Rel(string i1, string i2) => Iv(i1).RelationTo(Iv(i2), Comparer<int>.Default);

        [TestMethod]
        public void FirstBeforeSecondRelation()
        {
            Assert.AreEqual(
                new IntervalRelation<int>.Disjunct { First = Iv("1..4"), Second = Iv("5..7") }, 
                Rel("1..4", "5..7"));
            Assert.AreEqual(
                new IntervalRelation<int>.Disjunct { First = Iv("1..4"), Second = Iv("5..7") },
                Rel("5..7", "1..4"));
        }

        [TestMethod]
        public void FirstTouchesSecondRelation()
        {
            Assert.AreEqual(
                new IntervalRelation<int>.Touching { First = Iv("1..4"), Second = Iv("4..7") },
                Rel("1..4", "4..7"));
            Assert.AreEqual(
                new IntervalRelation<int>.Touching { First = Iv("1..4"), Second = Iv("4..7") },
                Rel("4..7", "1..4"));
        }

        [TestMethod]
        public void FirstStartsSecondRelation()
        {
            Assert.AreEqual(
                new IntervalRelation<int>.Starting { Overlap = Iv("4..6"), Disjunct = Iv("6..8") },
                Rel("4..8", "4..6"));
            Assert.AreEqual(
                new IntervalRelation<int>.Starting { Overlap = Iv("4..6"), Disjunct = Iv("6..8") },
                Rel("4..6", "4..8"));
        }

        [TestMethod]
        public void FirstFinishesSecondRelation()
        {
            Assert.AreEqual(
                new IntervalRelation<int>.Finishing { Disjunct = Iv("4..6"), Overlap = Iv("6..8") },
                Rel("6..8", "4..8"));
            Assert.AreEqual(
                new IntervalRelation<int>.Finishing { Disjunct = Iv("4..6"), Overlap = Iv("6..8") },
                Rel("4..8", "6..8"));
        }

        [TestMethod]
        public void SingletonIntersectionRelation()
        {
            Assert.AreEqual(
                new IntervalRelation<int>.Overlapping 
                { 
                    FirstDisjunct = Iv("4..6"), 
                    Overlap = Iv("6..=6"), 
                    SecondDisjunct = new Interval<int>(LowerBound<int>.Exclusive(6), UpperBound<int>.Exclusive(8)) },
                Rel("4..=6", "6..8"));
            Assert.AreEqual(
                new IntervalRelation<int>.Overlapping
                {
                    FirstDisjunct = Iv("4..6"),
                    Overlap = Iv("6..=6"),
                    SecondDisjunct = new Interval<int>(LowerBound<int>.Exclusive(6), UpperBound<int>.Exclusive(8))
                },
                Rel("6..8", "4..=6"));
        }

        [TestMethod]
        public void FirstContainsSecondRelation()
        {
            Assert.AreEqual(
                new IntervalRelation<int>.Containing { FirstDisjunct = Iv("2..4"), Contained = Iv("4..7"), SecondDisjunct = Iv("7..10") },
                Rel("2..10", "4..7"));
            Assert.AreEqual(
                new IntervalRelation<int>.Containing { FirstDisjunct = Iv("2..4"), Contained = Iv("4..7"), SecondDisjunct = Iv("7..10") },
                Rel("4..7", "2..10"));
        }

        [TestMethod]
        public void FirstIntersectsSecondRelation()
        {
            Assert.AreEqual(
                new IntervalRelation<int>.Overlapping { FirstDisjunct = Iv("2..4"), Overlap = Iv("4..7"), SecondDisjunct = Iv("7..9") },
                Rel("2..7", "4..9"));
            Assert.AreEqual(
                new IntervalRelation<int>.Overlapping { FirstDisjunct = Iv("2..4"), Overlap = Iv("4..7"), SecondDisjunct = Iv("7..9") },
                Rel("4..9", "2..7"));
        }
    }
}
