﻿using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Tree;
using CSharpier;
using CSharpier.DocPrinter;
using PrettierGML;
using System.Diagnostics;

void printTokens(string input)
{
    ICharStream stream = CharStreams.fromString(input);
    var lexer = new GameMakerLanguageLexer(stream);

    for (var token = lexer.NextToken(); !lexer.HitEOF; token = lexer.NextToken())
    {
        var name = lexer.Vocabulary.GetSymbolicName(token.Type);
        Console.WriteLine(name);
    }
    lexer.Reset();
}

void parse(string input)
{
    Stopwatch sw = Stopwatch.StartNew();

    ICharStream stream = CharStreams.fromString(input);
    var lexer = new GameMakerLanguageLexer(stream);
    var tokens = new CommonTokenStream(lexer);
    var parser = new GameMakerLanguageParser(tokens) { BuildParseTree = true };
    parser.Interpreter.PredictionMode = PredictionMode.SLL;
    IParseTree tree = parser.program();
    //Console.WriteLine(tree.ToStringTree());

    var printer = new VisitorAndPrinter(tokens);
    var doc = printer.Visit(tree);

    var serialized = DocSerializer.Serialize(doc);

    PrinterOptions options = new() { Width = 80 };

    var printed = DocPrinter.Print(doc, options, "\n");

    sw.Stop();
    Console.WriteLine($"Total Time: {sw.ElapsedMilliseconds.ToString()} ms");

    Console.WriteLine(serialized);
    Console.WriteLine(printed);
}

parse(
    @"
x = foo.bar(
        argument1,
        Argument2.foooooooooooooooooooooooooooooooooooooooooooooooooooooooooo
  .barrrrrrrrrrrrrrrr.bazzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz()
            .bar().baz.fooooooooo //comment
  .fooooooooooooooooooooooooooooooooooooooooooo
            .baazzzzzz().
//this comment
// another comment
/*some suspect*//*formatting*/
/*among other concerns*/
x().y(
                argument2()
                .bar() // this is bar
.baz.fooooooooo
               ).z().foooooooooooooooooooooooooooooooooooooooooooooooooooooooooo
  .barrrrrrrrrrrrrrrr.bazzzzzzzzzzzzzzz
    .barrrrrrrrrrrrrrrr.bazzzzzzzzzzzzzzz()
    .barrrrrrrrrrrrrrrr.bazzzzzzzzzzzzzzz
    );

x = Argument2.foooooooooooooooooooooooooooooooooooooooooooooooooooooooooo
  .barrrrrrrrrrrrrrrr.bazzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz
"
);
