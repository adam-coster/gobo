﻿using Antlr4.Runtime;
using PrettierGML.Nodes.PrintHelpers;
using PrettierGML.Printer.Document.DocTypes;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class ArrayExpression : GmlSyntaxNode
    {
        public List<GmlSyntaxNode> Elements => Children;

        public ArrayExpression(ParserRuleContext context, List<GmlSyntaxNode> elements)
            : base(context)
        {
            AsChildren(elements);
        }

        public override Doc Print(PrintContext ctx)
        {
            return DelimitedList.PrintInBrackets(ctx, "[", this, "]", ",");
        }
    }
}
