using UnityEngine;

[CreateAssetMenu(fileName = "GameRule", menuName = "Bellz+Merge/Game/GameRule")]
public class GameRule : ScriptableObject
{
    [SerializeField] private string _label;
    [TextArea(2, 8)] [SerializeField] private string _description;
    [SerializeField] private Sprite _reference;

    public string Label => _label;
    public string Description => _description;
    public Sprite Reference => _reference;
}
