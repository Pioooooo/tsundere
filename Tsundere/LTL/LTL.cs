using Antlr4.Runtime;
using Tsundere.Parser;
using Tsundere.TS;
using Tsundere.Visitors;

namespace Tsundere.LTL;

public abstract class LtlNode
{
    public static readonly True TrueNode = True.CreateInstance();
    public static readonly Negation FalseNode = new(TrueNode);
    public abstract List<LtlNode> Children { get; }
    private protected virtual bool IsClear => true;

    public abstract override string ToString();

    public string ToClearString() => IsClear ? ToString() : $"({ToString()})";

    public virtual LtlNode Neg() => new Negation(this);

    public static Negation Disjuncture(LtlNode l, LtlNode r) => new(new Conjuncture(l.Neg(), r.Neg()));

    public static Negation Imply(LtlNode l, LtlNode r) => new(new Conjuncture(l, r.Neg()));

    public static Until Eventually(LtlNode child) => new(TrueNode, child);

    public static Negation Always(LtlNode child) => new(LtlNode.Eventually(child.Neg()));
}

public class True : LtlNode
{
    public override List<LtlNode> Children { get; } = new();

    public override string ToString() => "true";

    private True()
    {
    }

    internal static True CreateInstance() => new();
}

public class Variable : LtlNode
{
    private readonly AtomicProposition _atomicProp;

    public Variable(AtomicProposition atomicProp) => _atomicProp = atomicProp;

    public override List<LtlNode> Children { get; } = new();

    public override string ToString() => _atomicProp.ToString();
}

public class Conjuncture : LtlNode
{
    private readonly LtlNode _l, _r;

    public Conjuncture(LtlNode l, LtlNode r)
    {
        _l = l;
        _r = r;
    }

    public override List<LtlNode> Children => new() {_l, _r};
    private protected override bool IsClear => false;

    public override string ToString() => $"{_l.ToClearString()} /\\ {_r.ToClearString()}";
}

public class Negation : LtlNode
{
    private readonly LtlNode _child;

    public Negation(LtlNode child) => _child = child;

    public override List<LtlNode> Children => new() {_child};
    public override string ToString() => $"!{_child.ToClearString()}";

    public override LtlNode Neg() => _child;
}

public class Next : LtlNode
{
    private readonly LtlNode _child;

    public Next(LtlNode child) => _child = child;

    public override List<LtlNode> Children => new() {_child};
    public override string ToString() => $"X{_child.ToClearString()}";
}

public class Until : LtlNode
{
    private readonly LtlNode _l, _r;

    public Until(LtlNode l, LtlNode r)
    {
        _l = l;
        _r = r;
    }

    public override List<LtlNode> Children => new() {_l, _r};
    private protected override bool IsClear => false;
    public override string ToString() => $"{_l.ToClearString()} U {_r.ToClearString()}";
}

public class Ltl
{
    private readonly LtlNode _root;

    private Ltl(LtlNode root) => _root = root;

    public static Ltl Parse(string input, TransitionSystem ts)
    {
        var antlrInputStream = new AntlrInputStream(input);
        var tsundereLexer = new TsundereLexer(antlrInputStream);
        var commonTokenStream = new CommonTokenStream(tsundereLexer);
        var tsundereParser = new TsundereParser(commonTokenStream);
        var context = tsundereParser.language();
        return new Ltl(new LtlBuilder(ts.PropsByString).VisitLanguage(context));
    }

    public override string ToString() => _root.ToString();
}