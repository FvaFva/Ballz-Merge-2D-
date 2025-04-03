using UnityEngine;

[CreateAssetMenu(fileName = "New field expander settings", menuName = "Bellz+Merge/Field/Settings", order = 51)]
public class FieldExpanderSettings : ScriptableObject
{
    [SerializeField] private int _waveCount;
    [SerializeField] public int _count;
    [SerializeField] public bool _isLoop;

    public int WaveCount => _waveCount;
    public int Count => _count;
    public bool IsLoop => _isLoop;
}
