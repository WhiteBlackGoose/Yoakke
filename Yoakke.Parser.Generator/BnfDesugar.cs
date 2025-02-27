﻿using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Yoakke.Parser.Generator.Ast;

namespace Yoakke.Parser.Generator
{
    internal static class BnfDesugar
    {
        public static Rule EliminateLeftRecursion(Rule rule)
        {
            if (rule.Ast is BnfAst.Alt alt)
            {
                var newAlt = EliminateLeftRecursionInAlternation(rule, alt);
                return new Rule(rule.Name, newAlt, rule.PublicApi) { VisualName = rule.VisualName };
            }
            return rule;
        }

        public static IList<Rule> GeneratePrecedenceParser(Rule rule, IList<PrecedenceEntry> precedenceTable)
        {
            // The resulting precedence rules should look like
            // RULE_N : (RULE_N OP RULE_(N+1)) {TR} | RULE_(N+1) for left-associative
            // RULE_N : (RULE_(N+1) OP RULE_N) {TR} | RULE_(N+1) for right-associative
            // And simply the passed-in rule as atomic

            var result = new List<Rule>();
            var atom = new Rule($"{rule.Name}_atomic", rule.Ast, false);
            result.Add(atom);
            for (int i = 0; i < precedenceTable.Count; ++i)
            {
                var prec = precedenceTable[i];
                var currentCall = new BnfAst.Call(i == 0 ? rule.Name : $"{rule.Name}_level{i}");
                var nextCall = new BnfAst.Call(i == precedenceTable.Count - 1 ? atom.Name : $"{rule.Name}_level{i + 1}");

                BnfAst? toAdd = null;
                foreach (var op in prec.Operators)
                {
                    var opNode = new BnfAst.Literal(op);
                    var seq = prec.Left
                        ? new BnfAst[] { currentCall, opNode, nextCall } 
                        : new BnfAst[] { nextCall, opNode, currentCall };
                    var alt = new BnfAst.Transform(new BnfAst.Seq(seq), prec.Method);
                    if (toAdd == null) toAdd = alt;
                    else toAdd = new BnfAst.Alt(toAdd, alt);
                }
                // Default is always stepping a level down
                if (toAdd == null) toAdd = nextCall;
                else toAdd = new BnfAst.Alt(toAdd, nextCall);

                result.Add(new Rule(currentCall.Name, toAdd, i == 0) { VisualName = rule.VisualName });
            }
            return result;
        }

        // Left-recursion //////////////////////////////////////////////////////

        /*
         We need to find alternatives that are left recursive
         The problem is that they might be inside transformations
         For example:
         A -> A + X => { ... }
            | A - Y => { ... }
            | C
            | D
         In this case we need to collect the first 2 alternatives (assuming same transformation)
         And we need to make it into:
         A -> FoldLeft((C | D)(X | Y)*)
         */

        private static BnfAst EliminateLeftRecursionInAlternation(Rule rule, BnfAst.Alt alt)
        {
            var alphas = new List<BnfAst>();
            var betas = new List<BnfAst>();
            IMethodSymbol? fold = null;
            foreach (var child in alt.Elements)
            {
                // If the inside has no transformation, we don't care
                if (child is not BnfAst.Transform transform)
                {
                    betas.Add(child);
                    continue;
                }
                // We found a left-recursive sequence inside an alternative, add it as alpha
                if (transform.Subexpr is BnfAst.Seq seq && TrySplitLeftRecursion(rule, seq, out var alpha))
                {
                    if (fold == null)
                    {
                        fold = transform.Method;
                    }
                    else if (!SymbolEqualityComparer.Default.Equals(fold, transform.Method))
                    {
                        throw new InvalidOperationException("Incompatible fold functions");
                    }
                    alphas.Add(alpha!);
                }
                else betas.Add(child);
            }
            if (alphas.Count == 0 || betas.Count == 0) return alt;
            // We have left-recursion
            var betaNode = betas.Count == 1 ? betas[0] : new BnfAst.Alt(betas);
            var alphaNode = alphas.Count == 1 ? alphas[0] : new BnfAst.Alt(alphas);
            return new BnfAst.FoldLeft(betaNode, alphaNode, fold!);
        }

        private static bool TrySplitLeftRecursion(Rule rule, BnfAst.Seq seq, [MaybeNullWhen(false)] out BnfAst? alpha)
        {
            if (seq.Elements[0] is BnfAst.Call call && call.Name == rule.Name)
            {
                // Is left recursive, split out the left-recursive part, make the rest be in alpha
                if (seq.Elements.Count == 2) alpha = seq.Elements[1];
                else alpha = new BnfAst.Seq(seq.Elements.Skip(1));
                return true;
            }
            else
            {
                alpha = null;
                return false;
            }
        }
    }
}
