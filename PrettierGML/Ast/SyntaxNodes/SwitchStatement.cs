﻿using Antlr4.Runtime;
using PrettierGML.Printer.Document.DocTypes;
using PrettierGML.Nodes.PrintHelpers;
using PrettierGML.Printer;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class SwitchStatement : GmlSyntaxNode
    {
        public GmlSyntaxNode Discriminant { get; set; }
        public GmlSyntaxNode Cases { get; set; }

        public SwitchStatement(
            ParserRuleContext context,
            GmlSyntaxNode discriminant,
            GmlSyntaxNode cases
        )
            : base(context)
        {
            Discriminant = AsChild(discriminant);
            Cases = AsChild(cases);
        }

        public override Doc Print(PrintContext ctx)
        {
            return Doc.Concat(
                Doc.Concat(
                    "switch",
                    " ",
                    Statement.EnsureExpressionInParentheses(ctx, Discriminant),
                    " "
                ),
                Cases.Print(ctx)
            );
        }
    }
}
