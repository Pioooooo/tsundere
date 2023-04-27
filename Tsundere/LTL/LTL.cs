using Antlr4.Runtime;
using Tsundere.BA;
using Tsundere.Parser;
using Tsundere.TS;
using Tsundere.Util;
using Tsundere.Visitors;

namespace Tsundere.LTL;

public abstract class LtlNode
{
    public static readonly True TrueNode = True.CreateInstance();
    public static readonly Negation FalseNode = new(TrueNode);
    protected abstract List<LtlNode> Children { get; }
    private protected virtual bool IsClear => true;

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == this.GetType() && GetHashCode() == obj.GetHashCode();
    }

    public abstract override int GetHashCode();

    public abstract override string ToString();

    public string ToClearString() => IsClear ? ToString() : $"({ToString()})";

    public virtual LtlNode Neg => new Negation(this);

    public ElementarySet Closure =>
        Children.SelectMany(c => c.Closure).Union(new List<LtlNode> { this }).ToElementarySet();

    public static Negation Disjuncture(LtlNode l, LtlNode r) => new(new Conjuncture(l.Neg, r.Neg));

    public static Negation Imply(LtlNode l, LtlNode r) => new(new Conjuncture(l, r.Neg));

    public static Until Eventually(LtlNode child) => new(TrueNode, child);

    public static Negation Always(LtlNode child) => new(Eventually(child.Neg));
}

public class True : LtlNode
{
    protected override List<LtlNode> Children { get; } = new();

    public override string ToString() => "true";

    private True()
    {
    }

    internal static True CreateInstance() => new();

    public override int GetHashCode() => "True".GetHashCode();
}

public class Variable : LtlNode
{
    private readonly AtomicProposition _atomicProp;

    public Variable(AtomicProposition atomicProp) => _atomicProp = atomicProp;

    protected override List<LtlNode> Children { get; } = new();

    public override string ToString() => _atomicProp.ToString();

    public override int GetHashCode() => HashCode.Combine("Variable", _atomicProp);
}

public class Conjuncture : LtlNode
{
    private readonly LtlNode _l, _r;

    public Conjuncture(LtlNode l, LtlNode r)
    {
        _l = l;
        _r = r;
    }

    protected override List<LtlNode> Children => new() { _l, _r };
    private protected override bool IsClear => false;

    public override string ToString() => $"{_l.ToClearString()} /\\ {_r.ToClearString()}";

    public override int GetHashCode() => HashCode.Combine("Conjuncture", _l) ^ HashCode.Combine("Conjuncture", _r);
}

public class Negation : LtlNode
{
    private readonly LtlNode _child;

    public Negation(LtlNode child) => _child = child;

    protected override List<LtlNode> Children => new() { _child };
    public override string ToString() => $"!{_child.ToClearString()}";

    public override LtlNode Neg => _child;

    public override int GetHashCode() => HashCode.Combine("Negation", _child);
}

public class Next : LtlNode
{
    private readonly LtlNode _child;

    public Next(LtlNode child) => _child = child;

    protected override List<LtlNode> Children => new() { _child };
    public override string ToString() => $"X{_child.ToClearString()}";

    public override int GetHashCode() => HashCode.Combine("Next", _child);
}

public class Until : LtlNode
{
    private readonly LtlNode _l, _r;

    public Until(LtlNode l, LtlNode r)
    {
        _l = l;
        _r = r;
    }

    protected override List<LtlNode> Children => new() { _l, _r };
    private protected override bool IsClear => false;
    public override string ToString() => $"{_l.ToClearString()} U {_r.ToClearString()}";

    public override int GetHashCode() => HashCode.Combine("Until", _l, _r);
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

    public Ltl Neg => new Ltl(_root.Neg);

    public ElementarySet Closure => _root.Closure;

    public override string ToString() => _root.ToString();
}