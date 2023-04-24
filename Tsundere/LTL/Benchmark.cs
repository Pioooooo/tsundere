using Tsundere.TS;
using Tsundere.Util;

namespace Tsundere.LTL;

public class Benchmark
{
    private readonly TransitionSystem _ts;
    private readonly List<Ltl> _globals = new();
    private readonly Dictionary<State, List<Ltl>> _locals = new();

    private List<State> States => _ts.States;

    public Benchmark(TransitionSystem ts, TextReader textReader)
    {
        _ts = ts;
        Parse(textReader);
    }

    private void Parse(TextReader textReader)
    {
        var numbers = textReader.ReadLine()!.Split();
        var nGlobal = int.Parse(numbers[0]);
        var nLocal = int.Parse(numbers[1]);
        for (var i = 0; i < nGlobal; i++) _globals.Add(Ltl.Parse(textReader.ReadLine()!, _ts));

        foreach (var tsState in _ts.States) _locals.Add(tsState, new List<Ltl>());

        for (var i = 0; i < nLocal; i++)
        {
            var strings = textReader.ReadLine()!.Split(null, 2);
            _locals[States[int.Parse(strings[0])]].Add(Ltl.Parse(strings[1], _ts));
        }
    }

    public override string ToString() =>
        $"{nameof(_ts)}: {_ts}\n" +
        $"{nameof(_globals)}: {_globals.DataString()}\n" +
        $"{nameof(_locals)}: {_locals.DataString()}";
}