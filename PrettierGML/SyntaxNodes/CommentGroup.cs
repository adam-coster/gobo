﻿using Antlr4.Runtime;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PrettierGML.Printer.DocTypes;

namespace PrettierGML.SyntaxNodes
{
    internal enum CommentType
    {
        Leading,
        Trailing,
        Dangling,
    }

    internal enum CommentPlacement
    {
        OwnLine,
        EndOfLine,
        Remaining,
    }

    /// <summary>
    /// Represents a sequence of comments with no line breaks between them.
    /// </summary>
    internal class CommentGroup
    {
        public string Text => string.Concat(Tokens.Select(t => t.Text));

        public int Start => CharacterRange.Start;
        public int End => CharacterRange.Stop;

        [JsonConverter(typeof(StringEnumConverter))]
        public CommentType Type { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public CommentPlacement Placement { get; set; }

        [JsonIgnore]
        public List<IToken> Tokens { get; init; }

        [JsonIgnore]
        public Range CharacterRange { get; set; }

        [JsonIgnore]
        public Range TokenRange { get; set; }

        [JsonIgnore]
        public GmlSyntaxNode? EnclosingNode { get; set; }

        [JsonIgnore]
        public GmlSyntaxNode? PrecedingNode { get; set; }

        [JsonIgnore]
        public GmlSyntaxNode? FollowingNode { get; set; }

        [JsonIgnore]
        public bool Printed { get; set; } = false;

        [JsonIgnore]
        public bool IsFormatCommand { get; init; }

        [JsonIgnore]
        public string? FormatCommandText { get; init; }

        private bool endsWithSingleLineComment = false;

        public const string FormatCommandPrefix = "fmt-";

        public CommentGroup(List<IToken> tokens, Range characterRange, Range tokenRange)
        {
            Tokens = tokens;
            CharacterRange = characterRange;
            TokenRange = tokenRange;

            for (var i = tokens.Count - 1; i >= 0; i--)
            {
                var token = tokens[i];

                if (token.Type == GameMakerLanguageLexer.SingleLineComment)
                {
                    endsWithSingleLineComment = true;

                    var trimmedText = token.Text[2..].Trim();

                    if (trimmedText.StartsWith(FormatCommandPrefix))
                    {
                        IsFormatCommand = true;
                        FormatCommandText = trimmedText[FormatCommandPrefix.Length..];
                    }

                    break;
                }
            }
        }

        public Doc Print()
        {
            if (Printed)
            {
                //throw new Exception("Comment printed twice: " + Text);
            }
            Printed = true;

            var parts = new List<Doc>();

            foreach (var token in Tokens)
            {
                if (token.Type == GameMakerLanguageLexer.SingleLineComment)
                {
                    parts.Add(PrintSingleLineComment(token.Text));
                }
                else if (token.Type == GameMakerLanguageLexer.MultiLineComment)
                {
                    parts.Add(PrintMultiLineComment(token.Text));
                    if (token != Tokens.Last())
                    {
                        parts.Add(" ");
                    }
                }
            }

            if (endsWithSingleLineComment || Placement is CommentPlacement.EndOfLine)
            {
                parts.Add(Doc.BreakParent);
                return Doc.EndOfLineComment(parts);
            }
            else
            {
                return Doc.InlineComment(parts);
            }
        }

        /// <summary>
        /// Prints multiple lines of comments together, accounting for whitespace and padding.
        /// Setting <paramref name="type"/> to "Dangling" will ensure no padding is added.
        /// </summary>
        public static Doc PrintGroups(PrintContext ctx, List<CommentGroup> groups, CommentType type)
        {
            if (groups.Count == 0)
            {
                return Doc.Null;
            }

            var groupDocs = new List<Doc>() { groups.First().Print() };

            // Add line breaks between comment groups
            foreach (var group in groups.Skip(1))
            {
                int lineBreaksBetween =
                    ctx.Tokens
                        .GetHiddenTokensToLeft(group.TokenRange.Start)
                        .Reverse()
                        .TakeWhile(token => !IsComment(token))
                        ?.Count(token => token.Type == GameMakerLanguageLexer.LineTerminator) ?? 0;

                for (var i = 0; i < Math.Min(lineBreaksBetween, 2); i++)
                {
                    groupDocs.Add(Doc.HardLine);
                }

                groupDocs.Add(group.Print());
            }

            var printedGroups = Doc.Concat(groupDocs);

            if (type == CommentType.Dangling)
            {
                return printedGroups;
            }

            var parts = new List<Doc>();

            // Add leading or trailing line breaks depending on type
            if (type == CommentType.Leading)
            {
                int trailingLineBreakCount =
                    ctx.Tokens
                        .GetHiddenTokensToRight(groups.Last().TokenRange.Stop)
                        ?.Count(token => token.Type == GameMakerLanguageLexer.LineTerminator) ?? 0;

                if (trailingLineBreakCount == 0)
                {
                    return Doc.Concat(printedGroups);
                }

                parts.Add(printedGroups);

                for (var i = 0; i < Math.Min(trailingLineBreakCount, 2); i++)
                {
                    parts.Add(Doc.HardLine);
                }
            }
            else
            {
                int leadingLineBreakCount =
                    ctx.Tokens
                        .GetHiddenTokensToLeft(groups.First().TokenRange.Start)
                        ?.Count(token => token.Type == GameMakerLanguageLexer.LineTerminator) ?? 0;

                if (leadingLineBreakCount == 0)
                {
                    return Doc.Concat(printedGroups);
                }

                for (var i = 0; i < Math.Min(leadingLineBreakCount, 2); i++)
                {
                    parts.Add(Doc.HardLine);
                }

                parts.Add(printedGroups);
            }

            return Doc.Concat(parts);
        }

        private static bool IsComment(IToken token)
        {
            return token.Type == GameMakerLanguageLexer.SingleLineComment
                || token.Type == GameMakerLanguageLexer.MultiLineComment;
        }

        private static bool IsWhiteSpace(IToken token)
        {
            return token.Type == GameMakerLanguageLexer.LineTerminator
                || token.Type == GameMakerLanguageLexer.WhiteSpaces;
        }

        public static Doc PrintSingleLineComment(string text)
        {
            for (var i = 0; i < Math.Min(4, text.Length); i++)
            {
                var character = text[i];

                if (character == '/')
                {
                    continue;
                }
                else if (character != ' ')
                {
                    return text[..i] + " " + text[i..];
                }
                else
                {
                    break;
                }
            }

            return text;
        }

        public static Doc PrintMultiLineComment(string text)
        {
            var lines = text.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)
                .Select(line => (Doc)line);

            return Doc.Join(Doc.HardLine, lines);
        }

        public override string ToString()
        {
            return string.Join(
                '\n',
                $"Text: {string.Concat(Tokens.Select(t => t.Text))}",
                $"Type: {Type}",
                $"Placement: {Placement}",
                $"Range: {CharacterRange}",
                $"Enclosing: {EnclosingNode?.Kind}",
                $"Preceding: {PrecedingNode?.Kind}",
                $"Following: {FollowingNode?.Kind}\n"
            );
        }
    }
}
