﻿using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;
using Yoakke.Collections.Compatibility;

namespace Yoakke.Parser.Generator.Ast
{
    partial class BnfAst
    {
        public class FoldLeft : BnfAst
        {
            public readonly BnfAst First;
            public readonly BnfAst Second;
            public readonly IMethodSymbol Method;

            public FoldLeft(BnfAst first, BnfAst second, IMethodSymbol method)
            {
                First = first;
                Second = second;
                Method = method;
            }

            public override bool Equals(BnfAst other) => other is FoldLeft fl
                && First.Equals(fl.First)
                && Second.Equals(fl.Second)
                && SymbolEqualityComparer.Default.Equals(Method, fl.Method);
            public override int GetHashCode() => HashCode.Combine(First, Second, Method);

            public override BnfAst Desugar() => new FoldLeft(First.Desugar(), Second.Desugar(), Method);

            public override string GetParsedType(RuleSet ruleSet, TokenKindSet tokens)
            {
                var firstType = First.GetParsedType(ruleSet, tokens);
                var mappedType = Method.ReturnType.ToDisplayString();
                if (firstType != mappedType) throw new InvalidOperationException("Incompatible folded types");
                return firstType;
            }
        }
    }
}
