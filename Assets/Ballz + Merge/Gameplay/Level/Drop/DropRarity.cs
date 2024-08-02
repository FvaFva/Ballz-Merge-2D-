using UnityEngine;

[CreateAssetMenu(fileName = "New rarity", menuName = "Bellz+Merge/Drop/DropRarity", order = 51)]
public class DropRarity : ScriptableObject
{
    [SerializeField] private Color _color;
    [SerializeField] private int _countInPool;

    public Color Color => _color;
    public int CountInPool => _countInPool;
}
