﻿using System;
using Yoakke.Collections.FiniteAutomata;

namespace Yoakke.Collections.RegEx
{
    partial class RegExAst
    {
        /// <summary>
        /// Represents a single literal character to match.
        /// </summary>
        public class Literal : RegExAst
        {
            /// <summary>
            /// The character to match.
            /// </summary>
            public readonly char Char;

            public Literal(char @char)
            {
                Char = @char;
            }

            public override bool Equals(RegExAst other) => other is Literal lit && Char == lit.Char;
            public override int GetHashCode() => Char.GetHashCode();

            public override RegExAst Desugar() => this;

            public override (State Start, State End) ThompsonConstruct(DenseNfa<char> denseNfa)
            {
                var start = denseNfa.NewState();
                var end = denseNfa.NewState();

                denseNfa.AddTransition(start, Char, end);

                return (start, end);
            }
        }
    }
}
