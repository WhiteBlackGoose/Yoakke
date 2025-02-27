﻿using System;
using System.Collections.Generic;
using Yoakke.Reporting.Present;
using Yoakke.Text;

namespace Yoakke.Reporting.Sample
{
    class MyHighlighter : ISyntaxHighlighter
    {
        public SyntaxHighlightStyle Style { get; set; } = SyntaxHighlightStyle.Default;

        public IReadOnlyList<ColoredToken> GetHighlightingForLine(ISourceFile sourceFile, int line)
        {
            if (line == 1)
            {
                return new ColoredToken[]
                {
                new ColoredToken(0, 4, TokenKind.Keyword),
                new ColoredToken(5, 3, TokenKind.Name),
                };
            }
            else if (line == 2)
            {
                return new ColoredToken[]
                {
                new ColoredToken(4, 6, TokenKind.Keyword),
                new ColoredToken(11, 1, TokenKind.Literal),
                new ColoredToken(12, 1, TokenKind.Punctuation),
                };
            }
            return new ColoredToken[] { };
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var src = new SourceFile("test.txt", @"
func foo() {
    return 0;
}
");

            var presenter = new TextDiagnosticPresenter(Console.Error);
            presenter.SyntaxHighlighter = new MyHighlighter();

            var diag = new Diagnostic()
                .WithCode("E001")
                .WithSeverity(Severity.Error)
                .WithMessage("you made a mistake")
                .WithSourceInfo(new Location(src, new Text.Range(new Position(2, 4), 6)), Severity.Error, "mistake made here");

            presenter.Present(diag);
        }
    }
}
