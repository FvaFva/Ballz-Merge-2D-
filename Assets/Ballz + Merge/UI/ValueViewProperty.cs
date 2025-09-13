using System;
using UnityEngine;

[Serializable]
public struct ValueViewProperty
{
    public MonoBehaviour CounterObject;
    public IValueViewScore Counter;
    public ValueView ValueView;

    public void Init()
    {
        Counter = CounterObject.GetInterface<IValueViewScore>();
    }
}
