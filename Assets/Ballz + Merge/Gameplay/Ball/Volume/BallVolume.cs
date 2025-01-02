using UnityEngine;

[CreateAssetMenu(fileName = "New volume", menuName = "Bellz+Merge/Drop/BallVolume", order = 51)]
public class BallVolume : ScriptableObject
{
    [SerializeField] private BallVolumesTypes _type;
    [SerializeField] private BallVolumesSpecies _counting;
    [SerializeField] private Sprite _icon;
    [SerializeField] private string _name;
    [SerializeField] private string _description;
    [SerializeField] private bool _isReducible;

    public BallVolumesTypes Type => _type;
    public BallVolumesSpecies Species => _counting;
    public Sprite Icon => _icon;
    public string Name => _name;
    public string Description => _description;
    public bool IsReducible => _isReducible;
}
