using System.Diagnostics;
using Antlr4.Runtime;
using Tsundere.BA;
using Tsundere.Parser;
using Tsundere.TS;
using Tsundere.Util;
using Tsundere.Visitors;

namespace Tsundere.LTL;

public abstract class LtlNode : IComparable<LtlNode>
{
    public enum Consistency
    {
        True,
        False,
        Unknown,
        Contradict
    }

    public static readonly True TrueNode = True.Get();
    public static readonly Negation FalseNode = new(TrueNode);
    protected abstract List<LtlNode> Children { get; }
    public LtlNode C => Children[0];
    public LtlNode L => Children[0];
    public LtlNode R => Children[1];
    private int Depth => Children.Any() ? Children.Select(c => c.Depth).Max() + 1 : 1;
    private protected virtual bool IsClear => true;

    public virtual LtlNode Neg => new Negation(this);
    protected virtual LtlNode NonNeg => this;

    public FormulaSet HalfClosure =>
        Children.SelectMany(c => c.HalfClosure).Union(new[] { NonNeg }).ToFormulaSet();

    public int CompareTo(LtlNode? other) => Depth.CompareTo(other?.Depth);

    public virtual Consistency IsConsistent(FormulaSet f)
    {
        if (f.Contains(this)) return Consistency.True;
        return f.Contains(Neg) ? Consistency.Contradict : Consistency.Unknown;
    }

    public abstract override bool Equals(object? obj);
    public abstract override int GetHashCode();
    public abstract override string ToString();
    public string ToClearString() => IsClear ? ToString() : $"({ToString()})";
    public static Negation Disjuncture(LtlNode l, LtlNode r) => new(Conjuncture.Get(l.Neg, r.Neg));
    public static Negation Imply(LtlNode l, LtlNode r) => new(Conjuncture.Get(l, r.Neg));
    public static Until Eventually(LtlNode child) => Until.Get(TrueNode, child);
    public static Negation Always(LtlNode child) => new(Eventually(child.Neg));
}

public class True : LtlNode, IEquatable<True>
{
    private True()
    {
    }

    protected override List<LtlNode> Children { get; } = new();
    public bool Equals(True? other) => !ReferenceEquals(null, other);
    public static bool operator ==(True? left, True? right) => Equals(left, right);
    public static bool operator !=(True? left, True? right) => !Equals(left, right);
    public override Consistency IsConsistent(FormulaSet f) => Consistency.True;

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType();
    }

    public override string ToString() => "true";
    internal static True Get() => new();
    public override int GetHashCode() => "True".GetHashCode();
}

public class Variable : LtlNode, IEquatable<Variable>
{
    private static readonly Dictionary<string, Variable> StringToVar = new();
    public readonly AtomicProposition AtomicProp;
    private Variable(AtomicProposition atomicProp) => AtomicProp = atomicProp;
    protected override List<LtlNode> Children { get; } = new();

    public bool Equals(Variable? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return AtomicProp.Equals(other.AtomicProp);
    }

    public static bool operator ==(Variable? left, Variable? right) => Equals(left, right);
    public static bool operator !=(Variable? left, Variable? right) => !Equals(left, right);

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((Variable)obj);
    }

    public static Variable Get(string name) =>
        StringToVar.TryGetValue(name, out var value)
            ? value
            : StringToVar[name] = new Variable(AtomicProposition.Get(name));

    public override string ToString() => AtomicProp.ToString();
    public override int GetHashCode() => HashCode.Combine("Variable", AtomicProp);
}

public class Conjuncture : LtlNode, IEquatable<Conjuncture>
{
    private readonly LtlNode _l, _r;

    private Conjuncture(LtlNode l, LtlNode r)
    {
        _l = l;
        _r = r;
    }

    protected override List<LtlNode> Children => new() { _l, _r };
    private protected override bool IsClear => false;

    public bool Equals(Conjuncture? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return (_l.Equals(other._l) && _r.Equals(other._r)) || (_l.Equals(other._r) && _r.Equals(other._l));
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((Conjuncture)obj);
    }

    public static bool operator ==(Conjuncture? left, Conjuncture? right) => Equals(left, right);
    public static bool operator !=(Conjuncture? left, Conjuncture? right) => !Equals(left, right);
    public static Conjuncture Get(LtlNode l, LtlNode r) => new(l, r);

    public override Consistency IsConsistent(FormulaSet f)
    {
        var baseConsistent = base.IsConsistent(f);
        if (baseConsistent != Consistency.Unknown) return baseConsistent;
        var consistent = true;
        foreach (var c in Children)
            switch (c.IsConsistent(f))
            {
                case Consistency.True:
                    break;
                case Consistency.False:
                    throw new UnreachableException();
                case Consistency.Unknown:
                    consistent = false;
                    break;
                case Consistency.Contradict:
                    return Consistency.False;
                default:
                    throw new UnreachableException();
            }

        return consistent ? Consistency.True : Consistency.Unknown;
    }

    public override string ToString() => $"{_l.ToClearString()} /\\ {_r.ToClearString()}";
    public override int GetHashCode() => HashCode.Combine("Conjuncture", _l) ^ HashCode.Combine("Conjuncture", _r);
}

public class Negation : LtlNode, IEquatable<Negation>
{
    private readonly LtlNode _child;
    public Negation(LtlNode child) => _child = child;
    protected override List<LtlNode> Children => new() { _child };
    public override LtlNode Neg => _child;
    protected override LtlNode NonNeg => _child;

    public bool Equals(Negation? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return _child.Equals(other._child);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((Negation)obj);
    }

    public static bool operator ==(Negation? left, Negation? right) => Equals(left, right);
    public static bool operator !=(Negation? left, Negation? right) => !Equals(left, right);

    public override Consistency IsConsistent(FormulaSet f)
    {
        var baseConsistent = base.IsConsistent(f);
        if (baseConsistent != Consistency.Unknown) return baseConsistent;
        return _child.IsConsistent(f) switch
        {
            Consistency.True => Consistency.False,
            Consistency.False => throw new UnreachableException(),
            Consistency.Unknown => Consistency.Unknown,
            Consistency.Contradict => Consistency.True,
            _ => throw new UnreachableException()
        };
    }

    public override string ToString() => $"!{_child.ToClearString()}";
    public override int GetHashCode() => HashCode.Combine("Negation", _child);
}

public class Next : LtlNode, IEquatable<Next>
{
    private readonly LtlNode _child;
    private Next(LtlNode child) => _child = child;
    protected override List<LtlNode> Children => new() { _child };

    public bool Equals(Next? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return _child.Equals(other._child);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((Next)obj);
    }

    public static bool operator ==(Next? left, Next? right) => Equals(left, right);
    public static bool operator !=(Next? left, Next? right) => !Equals(left, right);
    public static Next Get(LtlNode child) => new(child);
    public override string ToString() => $"X{_child.ToClearString()}";
    public override int GetHashCode() => HashCode.Combine("Next", _child);
}

public class Until : LtlNode, IEquatable<Until>
{
    private readonly LtlNode _l, _r;

    private Until(LtlNode l, LtlNode r)
    {
        _l = l;
        _r = r;
    }

    protected override List<LtlNode> Children => new() { _l, _r };
    private protected override bool IsClear => false;

    public bool Equals(Until? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return _l.Equals(other._l) && _r.Equals(other._r);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((Until)obj);
    }

    public static bool operator ==(Until? left, Until? right) => Equals(left, right);
    public static bool operator !=(Until? left, Until? right) => !Equals(left, right);
    public static Until Get(LtlNode l, LtlNode r) => new(l, r);

    public override Consistency IsConsistent(FormulaSet f)
    {
        var baseConsistent = base.IsConsistent(f);
        if (baseConsistent != Consistency.Unknown) return baseConsistent;
        return _r.IsConsistent(f) switch
        {
            Consistency.True => Consistency.True,
            Consistency.False => throw new UnreachableException(),
            Consistency.Unknown => Consistency.Unknown,
            Consistency.Contradict => _l.IsConsistent(f) switch
            {
                Consistency.True => Consistency.Unknown,
                Consistency.False => throw new UnreachableException(),
                Consistency.Unknown => Consistency.Unknown,
                Consistency.Contradict => Consistency.False,
                _ => throw new UnreachableException()
            },
            _ => throw new UnreachableException()
        };
    }

    public override string ToString() => $"{_l.ToClearString()} U {_r.ToClearString()}";
    public override int GetHashCode() => HashCode.Combine("Until", _l, _r);
}

public class Ltl
{
    private readonly LtlNode _root;
    private Ltl(LtlNode root) => _root = root;
    public Ltl Neg => new(_root.Neg);
    public FormulaSet HalfClosure => _root.HalfClosure;

    public static Ltl Parse(string input)
    {
        var antlrInputStream = new AntlrInputStream(input);
        var tsundereLexer = new TsundereLexer(antlrInputStream);
        var commonTokenStream = new CommonTokenStream(tsundereLexer);
        var tsundereParser = new TsundereParser(commonTokenStream);
        var context = tsundereParser.language();
        return new Ltl(new LtlBuilder().VisitLanguage(context));
    }

    public override string ToString() => _root.ToString();

    public bool IsConsistent(FormulaSet f) => _root.IsConsistent(f) switch
    {
        LtlNode.Consistency.True => true,
        LtlNode.Consistency.False => throw new UnreachableException(),
        LtlNode.Consistency.Unknown => throw new UnreachableException(),
        LtlNode.Consistency.Contradict => false,
        _ => throw new UnreachableException()
    };
}