using UnityEngine;

[CreateAssetMenu(fileName = "New volume", menuName = "Bellz+Merge/Drop/BallVolume", order = 51)]
public class BallVolume : ScriptableObject
{
    [SerializeField] private BallVolumesTypes _waveDropType;
    [SerializeField] private Sprite _icon;
    [SerializeField] private string _name;
    [SerializeField] private string _description;

    public BallVolumesTypes WaveDropType => _waveDropType;
    public Sprite Icon => _icon;
    public string Name => _name;
    public string Description => _description;
}
