using Tsundere.LTL;

namespace Tsundere.BA;

public class FormulaSet : HashSet<LtlNode>, IEquatable<FormulaSet>
{
    public FormulaSet()
    {
    }

    public FormulaSet(IEnumerable<LtlNode> collection) : base(collection)
    {
    }

    public bool Equals(FormulaSet? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return SetEquals(other);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((FormulaSet)obj);
    }

    public static bool operator ==(FormulaSet? left, FormulaSet? right) => Equals(left, right);

    public static bool operator !=(FormulaSet? left, FormulaSet? right) => !Equals(left, right);

    public override int GetHashCode() => this.Aggregate(0, HashCode.Combine);
}