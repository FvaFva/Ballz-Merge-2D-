using System.Collections.Generic;

public static class DictionaryExtension
{
    public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> dict, IEnumerable<KeyValuePair<TKey, TValue>> items)
    {
        foreach (var item in items)
            dict[item.Key] = item.Value;
    }
}
