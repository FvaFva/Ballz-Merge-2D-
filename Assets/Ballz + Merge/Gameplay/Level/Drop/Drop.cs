using UnityEditor;
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

#if UNITY_EDITOR
        if (_volume != null && _rarity != null)
            RenameAsset($"[{_volume.Name}] - [{_rarity.name}]");
#endif
    }

    public float GetRandomCount()
    {
        int stepCount = Mathf.FloorToInt((_countDiceMax - _countDiceMin) / DiceStep);
        int randomStep = Random.Range(0, stepCount + 1);
        return _countDiceMin + randomStep * DiceStep;
    }

    private float Standardize(float value) => Mathf.FloorToInt(value / DiceStep) * DiceStep;

#if UNITY_EDITOR
    private void RenameAsset(string newName)
    {
        if(newName == name) 
            return;

        string assetPath = AssetDatabase.GetAssetPath(this);

        if (string.IsNullOrEmpty(assetPath))
        {
            Debug.LogWarning("Asset path is null or empty.");
            return;
        }

        string result = AssetDatabase.RenameAsset(assetPath, newName);

        if (!string.IsNullOrEmpty(result))
        {
            Debug.LogError("Error renaming asset: " + result);
        }
        else
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
        }
    }
#endif
}