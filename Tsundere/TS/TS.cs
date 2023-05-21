using Tsundere.BA;
using Tsundere.Util;

namespace Tsundere.TS;

public class Named
{
    protected Named(string name = "") => Name = name;
    protected virtual string Name { get; }
    public override string ToString() => Name;
}

public class State : Named
{
    public bool Init;

    public State(string name) : base(name)
    {
    }
}

public class Action : Named
{
    public Action(string name) : base(name)
    {
    }
}

public class AtomicProposition : Named
{
    private static readonly Dictionary<string, AtomicProposition> StringToProps = new();

    private AtomicProposition(string name) : base(name)
    {
    }

    public static AtomicProposition Get(string name) =>
        StringToProps.TryGetValue(name, out var value)
            ? value
            : StringToProps[name] = new AtomicProposition(name);
}

public class Transition : Named
{
    public readonly Action Act;
    public readonly State From, To;

    public Transition(State from, State to, Action act)
    {
        From = from;
        To = to;
        Act = act;
    }

    protected override string Name => $"{From}->{To}:{Act}";
}

public class TransitionSystem
{
    private List<Action> _actions = new();
    private List<AtomicProposition> _atomicProps = new();
    private Dictionary<State, HashSet<AtomicProposition>> _label = new();
    private Dictionary<State, HashSet<Transition>> _transitions = new();
    public List<State> States = new();

    private TransitionSystem()
    {
    }

    private List<State> InitStates => States.FindAll(state => state.Init);

    public static bool Parse(TextReader textReader, out TransitionSystem ts)
    {
        try
        {
            ts = new TransitionSystem();

            string[] NextStrings() =>
                textReader.ReadLine()!.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);

            var numbers = NextStrings();
            var nStates = int.Parse(numbers[0]);
            var nTransitions = int.Parse(numbers[1]);
            for (var i = 0; i < nStates; i++)
            {
                ts.States.Add(new State(i.ToString()));
                ts._transitions.Add(ts.States[i], new HashSet<Transition>());
                ts._label.Add(ts.States[i], new HashSet<AtomicProposition>());
            }

            var initStates = NextStrings();
            foreach (var initState in initStates) ts.States[int.Parse(initState)].Init = true;

            var acts = NextStrings();
            foreach (var act in acts) ts._actions.Add(new Action(act));

            var atomicProps = NextStrings();
            foreach (var atomicProp in atomicProps) ts._atomicProps.Add(AtomicProposition.Get(atomicProp));

            for (var i = 0; i < nTransitions; i++)
            {
                var transition = NextStrings();
                var from = ts.States[int.Parse(transition[0])];
                var act = ts._actions[int.Parse(transition[1])];
                var to = ts.States[int.Parse(transition[2])];
                ts._transitions[from].Add(new Transition(from, to, act));
            }

            for (var i = 0; i < nStates; i++)
                foreach (var label in NextStrings())
                    ts._label[ts.States[i]].Add(ts._atomicProps[int.Parse(label)]);

            return true;
        }
        catch (Exception)
        {
            Console.WriteLine("Bad Input. Check your TS.");
            ts = null!;
            return false;
        }
    }

    public (TransitionSystem ts, HashSet<HashSet<AtomicProposition>> f) Product(Nba nba)
    {
        var ts = new TransitionSystem { _actions = _actions };

        var stateMap = States
            .SelectMany(s => nba.States.Select(nbaState => (k: (s, nbaState),
                v: new State($"{s}***{nbaState}")
                {
                    Init = s.Init && nba.InitStates.Any(i =>
                        nba.Delta[i].Any(t =>
                            t.To == nbaState && t.Act.AtomicProps.SetEquals(_label[s].Intersect(nba.Symbols))))
                })))
            .ToDictionary(p => p.k, p => p.v);
        ts.States = stateMap.Values.ToList();

        ts._transitions = _transitions
            .SelectMany(p => nba.Delta.Select(nbaP =>
            (
                k: stateMap[(p.Key, nbaP.Key)],
                v: p.Value
                    .SelectMany(t => nbaP.Value
                        .Where(d => d.Act.AtomicProps.SetEquals(_label[t.To].Intersect(nba.Symbols)))
                        .Select(d => new Transition(stateMap[(t.From, d.From)], stateMap[(t.To, d.To)], t.Act)))
                    .ToHashSet()
            ))).ToDictionary(p => p.k, p => p.v);

        var nbaStateToAp = nba.States.ToDictionary(s => s, s => AtomicProposition.Get(s.ToString()));
        ts._atomicProps = nbaStateToAp.Values.ToList();

        ts._label = stateMap.ToDictionary(p => p.Value,
            p => new HashSet<AtomicProposition> { nbaStateToAp[p.Key.nbaState] });
        return (ts, nba.F.Select(s => new HashSet<AtomicProposition> { nbaStateToAp[s] }).ToHashSet());
    }

    public (bool persistent, List<State>? counter) Persistent(HashSet<HashSet<AtomicProposition>> f)
    {
        var i = InitStates.ToHashSet();
        var r = new HashSet<State>();
        var u = new Stack<State>();
        var t = new HashSet<State>();
        var v = new Stack<State>();
        var cycleFound = false;

        void CycleCheck(State s)
        {
            v.Push(s);
            t.Add(s);
            do
            {
                var cur = v.Peek();
                var post = _transitions[cur].Select(trans => trans.To).ToHashSet();
                if (post.Contains(s))
                {
                    cycleFound = true;
                    v.Push(s);
                }
                else
                {
                    post.RemoveWhere(p => t.Contains(p));
                    if (post.Any())
                    {
                        var next = post.First();
                        v.Push(next);
                        t.Add(next);
                    }
                    else
                    {
                        v.Pop();
                    }
                }
            } while (v.Any() && !cycleFound);
        }

        void ReachableCycle(State s)
        {
            u.Push(s);
            r.Add(s);
            i.Remove(s);
            do
            {
                var cur = u.Peek();
                var post = _transitions[cur].Select(trans => trans.To).Except(r).ToList();
                if (post.Any())
                {
                    var next = post.First();
                    u.Push(next);
                    r.Add(next);
                    i.Remove(next);
                }
                else
                {
                    u.Pop();
                    if (f.Any(l => l.SetEquals(_label[cur]))) CycleCheck(cur);
                }
            } while (u.Any() && !cycleFound);
        }

        while (i.Any() && !cycleFound) ReachableCycle(i.First());

        return (!cycleFound, cycleFound ? u.Reverse().Concat(v.Reverse()).ToList() : null);
    }

    public override string ToString() =>
        $"{nameof(_transitions)}: {_transitions.DataString("\n")}\n" +
        $"{nameof(_label)}: {_label.DataString()}\n" +
        $"{nameof(InitStates)}: {InitStates.DataString()}";
}