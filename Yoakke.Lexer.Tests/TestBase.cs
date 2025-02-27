﻿using Yoakke.Text;

namespace Yoakke.Lexer.Tests
{
    public abstract class TestBase<TKind> where TKind : notnull
    {
        protected static Token<TKind> Token(string value, TKind tt, Range r) => new(r, value, tt);

        protected static Range Range((int Line, int Column) p1, (int Line, int Column) p2) =>
            new(new Position(p1.Line, p1.Column), new Position(p2.Line, p2.Column));

        protected static Range Range((int Line, int Column) p1, int len) =>
            new(new Position(p1.Line, p1.Column), len);
    }
}
