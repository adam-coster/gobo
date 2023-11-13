﻿using Antlr4.Runtime;
using PrettierGML.Printer.DocTypes;
using PrettierGML.SyntaxNodes.PrintHelpers;

namespace PrettierGML.SyntaxNodes.Gml
{
    internal sealed class RepeatStatement : GmlSyntaxNode
    {
        public GmlSyntaxNode Test { get; set; }
        public GmlSyntaxNode Body { get; set; }

        public RepeatStatement(ParserRuleContext context, GmlSyntaxNode test, GmlSyntaxNode body)
            : base(context)
        {
            Test = AsChild(test);
            Body = AsChild(body);
        }

        public override Doc PrintNode(PrintContext ctx)
        {
            return Statement.PrintControlFlowStatement(ctx, "repeat", Test, Body);
        }
    }
}
