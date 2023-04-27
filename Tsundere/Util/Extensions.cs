using Tsundere.BA;
using Tsundere.LTL;

namespace Tsundere.Util;

public static class StringExtension
{
    public static string DataString<TVal>(this IEnumerable<TVal> iEnumerable) => string.Join(", ", iEnumerable);

    public static string DataString<TKey, TVal>(this Dictionary<TKey, HashSet<TVal>> dict) where TKey : notnull =>
        $"[{string.Join(", ", dict.Select(pair => $"{{{pair.Key}, [{pair.Value.DataString()}]}}"))}]";

    public static string DataString<TKey, TVal>(this Dictionary<TKey, List<TVal>> dict) where TKey : notnull =>
        $"[{string.Join(", ", dict.Select(pair => $"{{{pair.Key}, [{pair.Value.DataString()}]}}"))}]";
}

public static class EnumerableExtension
{
    public static ElementarySet ToElementarySet(this IEnumerable<LtlNode> source) => new(source.ToHashSet());
}