using Tsundere.BA;
using Tsundere.TS;
using Tsundere.Util;

namespace Tsundere.LTL;

public class Benchmark
{
    private readonly List<Ltl> _globals = new();
    private readonly List<(State s, Ltl ltl)> _locals = new();
    private readonly TransitionSystem _ts;

    private Benchmark(TransitionSystem ts) => _ts = ts;

    private List<State> States => _ts.States;

    public static bool Parse(TransitionSystem ts, TextReader textReader, out Benchmark benchmark)
    {
        try
        {
            benchmark = new Benchmark(ts);
            var numbers = textReader.ReadLine()!.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
            var nGlobal = int.Parse(numbers[0]);
            var nLocal = int.Parse(numbers[1]);
            for (var i = 0; i < nGlobal; i++) benchmark._globals.Add(Ltl.Parse(textReader.ReadLine()!));

            for (var i = 0; i < nLocal; i++)
            {
                var strings = textReader.ReadLine()!.Split((char[]?)null, 2, StringSplitOptions.RemoveEmptyEntries);
                benchmark._locals.Add((benchmark.States[int.Parse(strings[0])], Ltl.Parse(strings[1])));
            }

            return true;
        }
        catch (Exception)
        {
            Console.WriteLine("Bad Input. Check your benchmark.");
            benchmark = null!;
            return false;
        }
    }

    public void Test(bool print = false)
    {
        foreach (var ltl in _globals) Test(ltl, print);

        foreach (var state in _ts.States) state.Init = false;
        foreach (var (s, ltl) in _locals)
        {
            s.Init = true;
            Test(ltl, print);
            s.Init = false;
        }
    }

    private void Test(Ltl ltl, bool print = false)
    {
        var gnba = new Gnba(ltl.Neg);
        if (print) Console.WriteLine($"GNBA:\n{gnba}\n");
        var nba = Nba.FromGnba(gnba);
        if (print) Console.WriteLine($"NBA:\n{nba}\n");
        var (product, f) = _ts.Product(nba);
        if (print)
        {
            Console.WriteLine($"Product:\n{product}\n");
            Console.WriteLine($"F: {f.DataStringLh()}\n");
        }

        var (persistent, counter) = product.Persistent(f);
        Console.WriteLine(persistent ? 1 : 0);
        if (!persistent && print) Console.WriteLine(counter!.DataString());
    }

    public override string ToString() =>
        $"{nameof(_ts)}: {_ts}\n" +
        $"{nameof(_globals)}: {_globals.DataString()}\n" +
        $"{nameof(_locals)}: {_locals.DataString()}";
}