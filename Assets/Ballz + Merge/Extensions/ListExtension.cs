using System.Collections.Generic;

public static class ListExtension
{
    public static T TakeRandom<T>(this List<T> list)
    { 
        if(list.Count == 0)
            return default(T);

        T element = list[UnityEngine.Random.Range(0, list.Count)];
        list.Remove(element);
        return element;
    }
}