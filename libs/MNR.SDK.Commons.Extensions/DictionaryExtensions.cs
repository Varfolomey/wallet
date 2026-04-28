namespace MNR.SDK.Commons.Extensions;

public static class DictionaryExtensions
{
    public static T ValueOrDefault<TK, T>(this Dictionary<TK, T> dictionary, TK key, T defaultT = default)
    {
        return dictionary?.ContainsKey(key) ?? false ? dictionary[key] : defaultT;
    }
}