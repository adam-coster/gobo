﻿using Antlr4.Runtime;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class UnaryExpression : GmlSyntaxNode
    {
        public string Operator { get; set; }
        public GmlSyntaxNode Argument { get; set; }
        public bool IsPrefix { get; set; }

        public UnaryExpression(
            ParserRuleContext context,
            string @operator,
            GmlSyntaxNode argument,
            bool isPrefix
        )
            : base(context)
        {
            Operator = @operator;
            Argument = AsChild(argument);
            IsPrefix = isPrefix;
        }

        public override Doc Print()
        {
            return Doc.Concat(Operator, Argument.Print());
        }
    }
}
