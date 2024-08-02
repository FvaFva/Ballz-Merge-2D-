using UnityEngine;

[CreateAssetMenu(fileName = "New drop", menuName = "Bellz+Merge/Drop/Drop", order = 51)]
public class Drop : ScriptableObject
{
    private const float DiceStep = 0.05f;
    private const float MaxValue = 1.5f;

    [SerializeField] private BallVolume _volume;
    [SerializeField] private DropRarity _rarity;
    [SerializeField, Range(DiceStep, MaxValue)] private float _countDiceMin;
    [SerializeField, Range(DiceStep, MaxValue)] private float _countDiceMax;

    public BallVolumesTypes WaveDropType => _volume.WaveDropType;
    public Sprite Icon => _volume.Icon;
    public string Name => _volume.Name;
    public string Description => _volume.Description;
    public Color Color => _rarity.Color;
    public int CountInPool => _rarity.CountInPool;

    private void OnValidate()
    {
        _countDiceMin = Standardize(_countDiceMin);
        _countDiceMax = Standardize(_countDiceMax);
    }

    public float GetRandomCount()
    {
        int stepCount = Mathf.FloorToInt((_countDiceMax - _countDiceMin) / DiceStep);
        int randomStep = Random.Range(0, stepCount + 1);
        return _countDiceMin + randomStep * DiceStep;
    }

    private float Standardize(float value) => Mathf.FloorToInt(value / DiceStep) * DiceStep;
}