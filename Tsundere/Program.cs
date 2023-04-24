using System.CommandLine;
using Antlr4.Runtime;
using Tsundere.LTL;
using Tsundere.Parser;
using Tsundere.TS;
using Tsundere.Visitors;

namespace Tsundere;

internal static class Program
{
    private static int Main(string[] args)
    {
        var tsOption = new Option<TextReader?>(
            name: "--ts",
            description: "The transition system file to read from.",
            isDefault: true,
            parseArgument: result =>
            {
                if (result.Tokens.Count == 0) return File.OpenText("TS.txt");

                var filePath = result.Tokens.Single().Value;
                if (File.Exists(filePath)) return File.OpenText(filePath);

                result.ErrorMessage = "The TS file does not exist";
                return null;
            }
        );
        tsOption.AddAlias("-t");
        var benchOption = new Option<TextReader?>(
            name: "--bench",
            description: "The LTL formula benchmark file to read from.",
            isDefault: true,
            parseArgument: result =>
            {
                if (result.Tokens.Count == 0) return File.OpenText("benchmark.txt");

                var filePath = result.Tokens.Single().Value;
                if (File.Exists(filePath)) return File.OpenText(filePath);

                result.ErrorMessage = "The benchmark file does not exist";
                return null;
            }
        );
        benchOption.AddAlias("-b");
        var outputOption = new Option<TextWriter?>(
            name: "--output",
            description: "The file to write to.",
            isDefault: true,
            parseArgument: result =>
            {
                if (result.Tokens.Count == 0) return Console.Out;

                var filePath = result.Tokens.Single().Value;
                return File.CreateText(filePath);
            }
        );
        outputOption.AddAlias("-o");
        var printStructureOption = new Option<bool>(
            name: "--structure",
            description: "Print LTL structure."
        );
        printStructureOption.AddAlias("-s");

        var rootCommand = new RootCommand("Model checker written in C#.");
        rootCommand.AddOption(tsOption);
        rootCommand.AddOption(benchOption);
        rootCommand.AddOption(outputOption);
        rootCommand.AddOption(printStructureOption);

        rootCommand.SetHandler(Work!, tsOption, benchOption, outputOption, printStructureOption);

        return rootCommand.Invoke(args);
    }

    private static void Work(TextReader tsReader, TextReader benchReader, TextWriter writer, bool printStructure)
    {
        Console.SetOut(writer);
        var ts = new TransitionSystem(tsReader);
        if (printStructure)
        {
            Console.WriteLine(ts);
        }

        var bench = new Benchmark(ts, benchReader);
        if (printStructure)
        {
            Console.Write(bench);
        }
    }
}