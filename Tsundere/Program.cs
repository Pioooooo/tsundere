using System.CommandLine;
using Tsundere.LTL;
using Tsundere.TS;

[assembly: CLSCompliant(false)]

namespace Tsundere;

internal static class Program
{
    private static int Main(string[] args)
    {
        var tsOption = new Option<string?>(
            name: "--ts",
            description: "The transition system file to read from.",
            isDefault: true,
            parseArgument: result =>
            {
                var filePath = result.Tokens.Any() ? result.Tokens.Single().Value : "TS.txt";
                if (File.Exists(filePath)) return filePath;

                result.ErrorMessage = "The TS file does not exist";
                return null;
            }
        );
        tsOption.AddAlias("-t");
        var benchOption = new Option<string?>(
            name: "--bench",
            description: "The LTL formula benchmark file to read from.",
            isDefault: true,
            parseArgument: result =>
            {
                var filePath = result.Tokens.Any() ? result.Tokens.Single().Value : "benchmark.txt";
                if (File.Exists(filePath)) return filePath;

                result.ErrorMessage = "The benchmark file does not exist";
                return null;
            }
        );
        benchOption.AddAlias("-b");
        var outputOption = new Option<string>(
            name: "--output",
            description: "The file to write to.",
            isDefault: true,
            parseArgument: result =>
            {
                if (result.Tokens.Count == 0) return "stdout";

                var filePath = result.Tokens.Single().Value;
                return "-" + filePath;
            }
        );
        outputOption.AddAlias("-o");
        var printStructureOption = new Option<bool>(
            name: "--verbose",
            description: "Output verbose information."
        );
        printStructureOption.AddAlias("-v");

        var rootCommand = new RootCommand("Model checker written in C#.");
        rootCommand.AddOption(tsOption);
        rootCommand.AddOption(benchOption);
        rootCommand.AddOption(outputOption);
        rootCommand.AddOption(printStructureOption);

        rootCommand.SetHandler(Work!, tsOption, benchOption, outputOption, printStructureOption);

        return rootCommand.Invoke(args);
    }

    private static void Work(string tsPath, string benchPath, string writerPath, bool printStructure)
    {
        var tsReader = File.OpenText(tsPath);
        var benchReader = File.OpenText(benchPath);
        var writer = writerPath == "stdout" ? Console.Out : File.CreateText(writerPath[1..]);
        Console.SetOut(writer);
        var ts = TransitionSystem.Parse(tsReader);

        var bench = new Benchmark(ts, benchReader);
        if (printStructure) Console.WriteLine(bench);
        bench.Test(printStructure);
    }
}