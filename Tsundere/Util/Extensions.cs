using Tsundere.BA;
using Tsundere.LTL;

namespace Tsundere.Util;

public static class StringExtension
{
    public static string DataString<TVal>(this IEnumerable<TVal> iEnumerable, string deli = ", ") =>
        string.Join(deli, iEnumerable);

    public static string DataStringLh<TVal>(this IEnumerable<HashSet<TVal>> iEnumerable, string deli = ", ",
        string deli1 = ", ") =>
        string.Join(deli, iEnumerable.Select(set => $"[{set.DataString(deli1)}]"));

    public static string DataString<TKey, TVal>(this Dictionary<TKey, HashSet<TVal>> dict, string deli = ", ",
        string deli1 = ", ") where TKey : notnull =>
        string.Join(deli, dict.Select(pair => $"{{{pair.Key}, [{pair.Value.DataString(deli1)}]}}"));

    public static string DataString<TKey, TVal>(this Dictionary<TKey, List<TVal>> dict, string deli = ", ",
        string deli1 = ", ") where TKey : notnull =>
        string.Join(deli, dict.Select(pair => $"{{{pair.Key}, [{pair.Value.DataString(deli1)}]}}"));
}

public static class EnumerableExtension
{
    public static FormulaSet ToFormulaSet(this IEnumerable<LtlNode> source) => new(source);
}

public static class MultiDictExtension
{
    public static TVal Get<TKey, TVal>(this Dictionary<TKey, TVal> dictionary, TKey key)
        where TKey : notnull where TVal : new() =>
        dictionary.TryGetValue(key, out var value) ? value : new TVal();
}