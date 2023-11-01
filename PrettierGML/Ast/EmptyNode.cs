﻿using PrettierGML.Printer;
using PrettierGML.Printer.Document.DocTypes;

namespace PrettierGML.Nodes
{
    internal class EmptyNode : GmlSyntaxNode
    {
        public static EmptyNode Instance { get; } = new();

        private EmptyNode() { }

        public override Doc Print(PrintContext ctx)
        {
            return Doc.Null;
        }
    }
}
