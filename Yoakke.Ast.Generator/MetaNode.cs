﻿using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Ast.Generator
{
    internal class MetaNode
    {
        public readonly INamedTypeSymbol Symbol;
        public readonly MetaNode? Parent;
        public string Name => Symbol.Name;
        public bool IsAbstract => Symbol.IsAbstract;
        public readonly IList<string> Nesting;
        public readonly IDictionary<string, MetaNode> Children = new Dictionary<string, MetaNode>();
        public readonly IList<(string Name, Type ReturnType)> Visitors = new List<(string Name, Type ReturnType)>();

        public MetaNode Root => Parent == null ? this : Parent.Root;

        private bool? implementEquality;
        public bool ImplementEquality
        {
            get => implementEquality == null ? Parent == null ? false : Parent.ImplementEquality : implementEquality.Value;
            set => implementEquality = value; 
        }

        public MetaNode(INamedTypeSymbol symbol, MetaNode? parent)
        {
            Symbol = symbol;
            Parent = parent;
            Nesting = GetNesting(Symbol);
        }

        private static IList<string> GetNesting(INamedTypeSymbol symbol)
        {
            var result = new List<string>();
            for (var parent = symbol.ContainingType; parent != null; parent = parent.ContainingType)
            {
                result.Insert(0, parent.Name);
            }
            return result;
        }
    }
}
