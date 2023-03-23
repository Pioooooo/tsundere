using System.CommandLine;
using Antlr4.Runtime;
using Tsundere.Parser;
using Tsundere.Visitors;

namespace Tsundere;

internal static class Program
{
    private static int Main(string[] args)
    {
        var inputOption = new Option<TextReader?>(
            name: "--input",
            description: "The file to read from.",
            isDefault: true,
            parseArgument: result =>
            {
                if (result.Tokens.Count == 0) return Console.In;

                var filePath = result.Tokens.Single().Value;
                if (File.Exists(filePath)) return new StreamReader(new FileStream(filePath, FileMode.Open));

                result.ErrorMessage = "Input file does not exist";
                return null;
            }
        );
        inputOption.AddAlias("-i");
        var outputOption = new Option<TextWriter?>(
            name: "--output",
            description: "The file to write to.",
            isDefault: true,
            parseArgument: result =>
            {
                if (result.Tokens.Count == 0) return Console.Out;

                var filePath = result.Tokens.Single().Value;
                return new StreamWriter(new FileStream(filePath, FileMode.OpenOrCreate));
            }
        );
        outputOption.AddAlias("-o");
        var printStructureOption = new Option<bool>(
            name: "--structure",
            description: "Print LTL structure."
        );
        printStructureOption.AddAlias("-s");

        var rootCommand = new RootCommand("Model checker written in C#.");
        rootCommand.AddOption(inputOption);
        rootCommand.AddOption(outputOption);
        rootCommand.AddOption(printStructureOption);

        rootCommand.SetHandler(Work!, inputOption, outputOption, printStructureOption);

        return rootCommand.Invoke(args);
    }

    private static void Work(TextReader reader, TextWriter writer, bool printStructure)
    {
        var antlrInputStream = new AntlrInputStream(reader);
        var tsundereLexer = new TsundereLexer(antlrInputStream);
        var commonTokenStream = new CommonTokenStream(tsundereLexer);
        var tsundereParser = new TsundereParser(commonTokenStream);

        var context = tsundereParser.language();
        if (printStructure)
        {
            var printer = new Printer(writer);
            printer.Visit(context);
        }
    }
}
