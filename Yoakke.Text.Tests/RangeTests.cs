﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Text.Tests
{
    [TestClass]
    public class RangeTests
    {
        public static IReadOnlyList<object[]> InvalidPositionPairs { get; } = new object[][]
        {
            new object[] { new Position(0, 1), new Position(0, 0) },
            new object[] { new Position(0, 3), new Position(0, 0) },
            new object[] { new Position(0, 3), new Position(0, 2) },
            new object[] { new Position(4, 3), new Position(3, 3) },
            new object[] { new Position(4, 3), new Position(3, 5) },
            new object[] { new Position(4, 3), new Position(4, 1) },
        };

        [DataTestMethod]
        [DynamicData(nameof(InvalidPositionPairs))]
        public void InvalidConstruction(Position start, Position end)
        {
            Assert.ThrowsException<ArgumentException>(() => new Range(start, end));
        }
    }
}
