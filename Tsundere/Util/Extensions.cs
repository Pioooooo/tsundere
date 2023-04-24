using System.Text;
using Microsoft.VisualBasic;

namespace Tsundere.Util;

public static class StringExtension
{
    public static string DataString<TKey, TVal>(this Dictionary<TKey, HashSet<TVal>> dict) where TKey : notnull
    {
        return $"[{String.Join(", ", dict.Select(pair => $"{{{pair.Key}, [{String.Join(", ", pair.Value)}]}}"))}]";
    }
}