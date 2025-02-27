﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Reporting.Present
{
    /// <summary>
    /// A single, colored token in the source code.
    /// </summary>
    public readonly struct ColoredToken
    {
        /// <summary>
        /// The start index of the token.
        /// </summary>
        public readonly int Start;
        /// <summary>
        /// The length of the token.
        /// </summary>
        public readonly int Length;
        /// <summary>
        /// The kind of the token.
        /// </summary>
        public readonly TokenKind Kind;

        public ColoredToken(int start, int length, TokenKind kind)
        {
            Start = start;
            Length = length;
            Kind = kind;
        }
    }
}
