using Tsundere.Util;

namespace Tsundere.TS;

public class Named
{
    public virtual string Name { get; }

    protected Named(string name = "") => Name = name;

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
    public AtomicProposition(string name) : base(name)
    {
    }
}

public class Transition : Named
{
    public readonly State From, To;
    public readonly Action Act;

    public Transition(State from, State to, Action act)
    {
        From = from;
        To = to;
        Act = act;
    }

    public override string Name => $"{From}->{To}:{Act}";
}

public class TransitionSystem
{
    public readonly List<State> States = new();
    private readonly List<Action> _actions = new();
    private readonly List<AtomicProposition> _atomicProps = new();
    private readonly Dictionary<State, HashSet<Transition>> _transitions = new();
    private readonly Dictionary<State, HashSet<AtomicProposition>> _label = new();

    private TransitionSystem()
    {
    }

    private List<State> InitStates => States.FindAll(state => state.Init);

    public Dictionary<string, AtomicProposition> PropsByString => _atomicProps.ToDictionary(ap => ap.Name, ap => ap);

    public static TransitionSystem Parse(TextReader textReader)
    {
        var ts = new TransitionSystem();
        string[] NextStrings() => textReader.ReadLine()!.Split();
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
        foreach (var atomicProp in atomicProps) ts._atomicProps.Add(new AtomicProposition(atomicProp));

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

        return ts;
    }

    public override string ToString() =>
        $"{nameof(_transitions)}: {_transitions.DataString()}\n" +
        $"{nameof(_label)}: {_label.DataString()}\n" +
        $"{nameof(InitStates)}: {InitStates.DataString()}";
}