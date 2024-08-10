using UnityEngine;

[CreateAssetMenu(fileName = "New volume", menuName = "Bellz+Merge/Drop/BallVolume", order = 51)]
public class BallVolume : ScriptableObject
{
    [SerializeField] private BallVolumesTypes _type;
    [SerializeField] private BallVolumeCountingTypes _counting;
    [SerializeField] private Sprite _icon;
    [SerializeField] private string _name;
    [SerializeField] private string _description;

    public BallVolumesTypes Type => _type;
    public BallVolumeCountingTypes Counting => _counting;
    public Sprite Icon => _icon;
    public string Name => _name;
    public string Description => _description;
}
