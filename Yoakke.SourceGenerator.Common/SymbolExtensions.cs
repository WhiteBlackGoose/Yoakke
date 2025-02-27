﻿using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Yoakke.SourceGenerator.Common
{
    public static class SymbolExtensions
    {
        public static bool IsNested(this ISymbol symbol) =>
            !SymbolEqualityComparer.Default.Equals(symbol.ContainingSymbol, symbol.ContainingNamespace);

        public static bool ImplementsInterface(this ITypeSymbol symbol, INamedTypeSymbol interf) =>
            symbol.AllInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(i, interf));

        public static bool ImplementsGenericInterface(this ITypeSymbol symbol, INamedTypeSymbol interf) =>
            ImplementsGenericInterface(symbol, interf, out var _);

        public static bool ImplementsGenericInterface(
            this ITypeSymbol symbol, 
            INamedTypeSymbol interf, 
            [MaybeNullWhen(false)] out IReadOnlyList<ITypeSymbol>? genericArgs)
        {
            if (SymbolEqualityComparer.Default.Equals(symbol.OriginalDefinition, interf))
            {
                genericArgs = ((INamedTypeSymbol)symbol).TypeArguments;
                return true;
            }
            var sub = symbol.AllInterfaces.FirstOrDefault(i => SymbolEqualityComparer.Default.Equals(i.OriginalDefinition, interf));
            if (sub != null)
            {
                genericArgs = sub.TypeArguments;
                return true;
            }
            genericArgs = null;
            return false;
        }
    }
}
