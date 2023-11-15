﻿using Antlr4.Runtime;
using PrettierGML.Printer.DocTypes;

namespace PrettierGML.SyntaxNodes.Gml
{
    internal sealed class DeleteStatement : GmlSyntaxNode
    {
        public GmlSyntaxNode Argument { get; set; }

        public DeleteStatement(TextSpan span, GmlSyntaxNode argument)
            : base(span)
        {
            Argument = AsChild(argument);
        }

        public override Doc PrintNode(PrintContext ctx)
        {
            return Doc.Concat("delete", " ", Argument.Print(ctx));
        }
    }
}
