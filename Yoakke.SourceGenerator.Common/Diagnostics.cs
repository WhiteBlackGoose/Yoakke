﻿using Microsoft.CodeAnalysis;

namespace Yoakke.SourceGenerator.Common
{
    internal static class Diagnostics
    {
        public static readonly DiagnosticDescriptor RequiredLibraryNotReferenced = new(
            id: "YKGEN001",
            title: "Required library not referenced",
            messageFormat: "Required library {0} is not referenced in the project",
            category: "Yoakke.Generator",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public static readonly DiagnosticDescriptor TypeDefinitionIsNotPartial = new(
            id: "YKGEN002",
            title: "Defined type is not partial",
            messageFormat: "The type definition {0} is not partial",
            category: "Yoakke.Generator",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public static readonly DiagnosticDescriptor SymbolIsNested = new(
            id: "YKGEN003",
            title: "Symbol is nested",
            messageFormat: "The symbol {0} is not partial",
            category: "Yoakke.Generator",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);
    }
}
