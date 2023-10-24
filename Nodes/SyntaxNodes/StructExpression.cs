﻿using Antlr4.Runtime;
using PrettierGML.Nodes.PrintHelpers;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class StructExpression : GmlSyntaxNode
    {
        public GmlSyntaxNode Properties { get; set; }

        public StructExpression(ParserRuleContext context, GmlSyntaxNode properties)
            : base(context)
        {
            Properties = AsChild(properties);
        }

        public override Doc Print(PrintContext ctx)
        {
            if (Properties.Children.Any())
            {
                return DelimitedList.PrintInBrackets(ctx, "{", Properties, "}", ",", true);
            }
            else
            {
                return EmptyStruct;
            }
        }

        public static string EmptyStruct => "{}";
    }
}