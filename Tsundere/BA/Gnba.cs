using System.Diagnostics;
using Tsundere.LTL;
using Tsundere.TS;
using Tsundere.Util;
using Action = Tsundere.TS.Action;

namespace Tsundere.BA;

public class GnbaAction : Action
{
    private static readonly Dictionary<FormulaSet, GnbaAction> VarsToAction = new();
    public readonly HashSet<AtomicProposition> AtomicProps;

    private GnbaAction(HashSet<AtomicProposition> ap) : base($"[{ap.DataString()}]") => AtomicProps = ap;

    public static GnbaAction Get(FormulaSet f)
    {
        var vars = f.Where(n => n is Variable).ToFormulaSet();
        return VarsToAction.TryGetValue(vars, out var value)
            ? value
            : VarsToAction[vars] = new GnbaAction(vars.Select(v => ((Variable)v).AtomicProp).ToHashSet());
    }
}

public class GnbaTransition : Transition
{
    public new GnbaAction Act => (GnbaAction)base.Act;

    public GnbaTransition(State from, State to, GnbaAction act) : base(from, to, act)
    {
    }
}

public class Gnba
{
    protected internal List<GnbaAction> Alphabet = new();
    protected internal Dictionary<State, HashSet<GnbaTransition>> Delta = new();
    protected internal List<HashSet<State>> F = new();
    protected internal List<State> States = new();
    protected internal HashSet<AtomicProposition> Symbols = new();

    protected Gnba()
    {
    }

    public Gnba(Ltl ltl)
    {
        var halfClosure = ltl.HalfClosure;
        var dict = halfClosure
            .GroupBy(l => l is Variable)
            .ToDictionary(p => p.Key, p => p.ToFormulaSet());
        var vars = dict.Get(true);
        Symbols = vars.Select(v => ((Variable)v).AtomicProp).ToHashSet();

        var rest = dict.Get(false);
        var (elementaryCover, powSet) = ElementaryCover(vars, rest);
        var varsToAlphabet = powSet
            .ToDictionary(f => f.Where(n => n is Variable).ToFormulaSet(), GnbaAction.Get);
        Alphabet = varsToAlphabet.Values.ToList();

        var stateToElementary = elementaryCover
            .ToDictionary(f => new State($"[{f.DataString()}]") { Init = ltl.IsConsistent(f) });
        States = stateToElementary.Keys.ToList();

        bool Satisfies(State s, LtlNode l) => stateToElementary[s].Contains(l);
        F = rest
            .Where(r => r is Until)
            .Select(u => States
                .Where(s => !Satisfies(s, u) || Satisfies(s, u.R))
                .ToHashSet())
            .DefaultIfEmpty(States.ToHashSet())
            .ToList();

        var typedDict = rest
            .GroupBy(l => l.GetType())
            .ToDictionary(p => p.Key, p => p.ToFormulaSet());
        var next = typedDict.Get(typeof(Next));
        var until = typedDict.Get(typeof(Until));


        Delta = States
            .ToDictionary(s => s, s => States
                .Where(n =>
                    next.All(nx => Satisfies(s, nx) == Satisfies(n, nx.C)) &&
                    until.All(u => Satisfies(s, u) == (Satisfies(s, u.R) || (Satisfies(s, u.L) && Satisfies(n, u)))))
                .Select(n => new GnbaTransition(s, n, GnbaAction.Get(stateToElementary[s])))
                .ToHashSet());
    }

    protected internal List<State> InitStates => States.FindAll(state => state.Init);

    private static (List<FormulaSet>, List<FormulaSet>) ElementaryCover(FormulaSet vars, FormulaSet rest)
    {
        var consistent = PowerSet(vars);
        if (rest.Remove(LtlNode.TrueNode)) consistent.ForEach(f => f.Add(LtlNode.TrueNode));
        var list = rest.ToList();
        list.Sort();
        return (list.Aggregate(consistent, (ret, cur) => ret.SelectMany(f => cur.IsConsistent(f) switch
        {
            LtlNode.Consistency.True => new[] { new FormulaSet(f) { cur } },
            LtlNode.Consistency.False => new[] { new FormulaSet(f) { cur.Neg } },
            LtlNode.Consistency.Unknown => new[] { new FormulaSet(f) { cur }, new FormulaSet(f) { cur.Neg } },
            LtlNode.Consistency.Contradict => throw new UnreachableException(), // Should not happen.
            _ => throw new UnreachableException()
        }).ToList()), consistent);
    }

    private static List<FormulaSet> PowerSet(FormulaSet set)
    {
        var ret = new List<FormulaSet> { new() };
        if (!set.Any()) return ret;
        ret.EnsureCapacity(1 << set.Count);
        return set.Aggregate(ret, (current, v) =>
            current.SelectMany(f => new[]
            {
                new FormulaSet(f) { v },
                new FormulaSet(f) { v.Neg }
            }).ToList());
    }

    public override string ToString() =>
        $"{nameof(Alphabet)}: {Alphabet.DataString()}\n" +
        $"{nameof(States)}: {States.DataString()}\n" +
        $"{nameof(InitStates)}: {InitStates.DataString()}\n" +
        $"{nameof(F)}: {F.DataStringLh()}\n" +
        $"{nameof(Delta)}: {Delta.DataString(",\n")}";
}

public class Nba : Gnba
{
    private Nba() => base.F.Add(new HashSet<State>());

    protected internal new HashSet<State> F
    {
        get => base.F[0];
        set => base.F[0] = value;
    }

    public static Nba FromGnba(Gnba gnba)
    {
        var n = gnba.F.Count;
        if (n == 0)
            return new Nba
            {
                Alphabet = gnba.Alphabet,
                Delta = gnba.Delta,
                F = new HashSet<State>(),
                States = gnba.States
            };
        var nba = new Nba { Alphabet = gnba.Alphabet, Symbols = gnba.Symbols };
        var stateMap = Enumerable.Range(0, n)
            .SelectMany(i => gnba.States
                .Select(s => (k: (s, i), v: new State($"{s}[{i}]") { Init = i == 0 && s.Init })))
            .ToDictionary(p => p.k, p => p.v);
        var f = gnba.F
            .SelectMany((f, i) => f.Select(s => stateMap[(s, i)]))
            .ToHashSet();
        nba.Delta = Enumerable.Range(0, n)
            .SelectMany(i => gnba.Delta.Select(p =>
            {
                var k = stateMap[(p.Key, i)];
                var c = f.Contains(k) ? 1 : 0;
                var v = p.Value
                    .Select(t => new GnbaTransition(k, stateMap[(t.To, (i + c) % n)], t.Act))
                    .ToHashSet();
                return (k, v);
            })).ToDictionary(p => p.k, p => p.v);
        nba.F = gnba.F[0].Select(s => stateMap[(s, 0)]).ToHashSet();
        nba.States = stateMap.Values.ToList();
        return nba;
    }

    public override string ToString() =>
        $"{nameof(Alphabet)}: {Alphabet.DataString()}\n" +
        $"{nameof(States)}: {States.DataString()}\n" +
        $"{nameof(InitStates)}: {InitStates.DataString()}\n" +
        $"{nameof(F)}: {F.DataString()}\n" +
        $"{nameof(Delta)}: {Delta.DataString(",\n")}";
}