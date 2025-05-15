using UnityEngine;

[CreateAssetMenu(fileName = "New field expander settings", menuName = "Bellz+Merge/Field/Settings", order = 51)]
public class FieldExpanderSettings : ScriptableObject
{
    [SerializeField] private int _countUntilSpawn;
    [SerializeField] public int _countOfSpawn;
    [SerializeField] public bool _isLoop;

    public int CountUntilSpawn => _countUntilSpawn;
    public int CountOfSpawn => _countOfSpawn;
    public bool IsLoop => _isLoop;
}
