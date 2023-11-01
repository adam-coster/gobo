﻿using Antlr4.Runtime;
using PrettierGML.Nodes.PrintHelpers;
using PrettierGML.Printer.Document.DocTypes;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class Block : GmlSyntaxNode
    {
        public List<GmlSyntaxNode> Statements => Children;

        public Block(ParserRuleContext context, List<GmlSyntaxNode> body)
            : base(context)
        {
            AsChildren(body);
        }

        public override Doc Print(PrintContext ctx)
        {
            return PrintInBlock(ctx, Statement.PrintStatements(ctx, Children));
        }

        public static Doc PrintInBlock(PrintContext ctx, Doc bodyDoc)
        {
            if (bodyDoc == Doc.Null)
            {
                return EmptyBlock;
            }

            Doc leadingLine =
                ctx.Options.BraceStyle == BraceStyle.NewLine ? Doc.HardLine : Doc.Null;

            return Doc.Concat(
                leadingLine,
                "{",
                Doc.Indent(Doc.HardLine, bodyDoc),
                Doc.HardLine,
                "}"
            );
        }

        public static Doc EmptyBlock => "{}";

        public override int GetHashCode()
        {
            if (Children.Count == 1)
            {
                return Children.First().GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }
    }
}
