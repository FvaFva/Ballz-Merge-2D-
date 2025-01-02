using UnityEditor;
using UnityEngine;

namespace BallzMerge.Gameplay.Level
{
    [CreateAssetMenu(fileName = "New drop", menuName = "Bellz+Merge/Drop/Drop", order = 51)]
    public class Drop : ScriptableObject
    {
        [SerializeField] private BallVolume _volume;
        [SerializeField] private DropRarity _rarity;

        public BallVolume Volume => _volume;
        public Sprite Icon => _volume.Icon;
        public string Name => _volume.Name;
        public string Description => _volume.Description;
        public Color Color => _rarity.Color;
        public int CountInPool => _rarity.CountInPool;
        public bool IsReducible => _volume.IsReducible;
        public DropRarity Rarity => _rarity;

        private void OnValidate()
        {
#if UNITY_EDITOR
            if (_volume != null && _rarity != null)
                RenameAsset($"[{_volume.Name}] - [{_rarity.name}]");
#endif
        }

#if UNITY_EDITOR
        private void RenameAsset(string newName)
        {
            if (newName == name)
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
}