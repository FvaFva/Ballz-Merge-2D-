using System.Collections.Generic;
using System.Linq;
using BallzMerge.Gameplay.Level;
using UnityEditor;
using UnityEngine;

namespace BallzMerge.Editor
{
    public class DropTab : IGameSettingsTab
    {
        private const int NameSpace = 200;
        private const int TypicalHight = 20;

        private AssetData<LevelSettings> _settings;
        private EditorService _service = new EditorService();
        private List<AssetData<BallVolumeOnHit>> _onHitVolumes;
        private List<AssetData<BallVolumePassive>> _passiveVolumes;
        private List<AssetData<DropRarity>> _rarities;
        private Dictionary<DropRarity, List<BallVolume>> _dropListDict;
        private Vector2 _scroll;

        public string Title => "Drop";

        public IGameSettingsTab LoadData(AssetData<LevelSettings> settings)
        {
            _settings = settings;
            _dropListDict = _settings.ScriptableObject.DropSettings.DropMap;
            _onHitVolumes = _service.LoadObjects<BallVolumeOnHit>();
            _passiveVolumes = _service.LoadObjects<BallVolumePassive>();
            _rarities = _service.LoadObjects<DropRarity>();
            _rarities = _rarities.OrderBy(r => r.ScriptableObject.Weight).ToList();

            foreach (var rarity in _rarities)
            {
                if (_dropListDict.ContainsKey(rarity.ScriptableObject) == false)
                    _dropListDict.Add(rarity.ScriptableObject, new List<BallVolume>());
            }

            return this;
        }

        public void OnGUI()
        {
            ShowHeader();
            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            ShowVolumes("On hit volumes", _onHitVolumes);
            ShowVolumes("Passive volumes", _passiveVolumes);
            EditorGUILayout.EndScrollView();
        }

        private void ShowHeader()
        {
            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            GUILayout.Label("Icon", GUILayout.Width(_service.IconSpace));
            _service.DrawLine(TypicalHight);
            GUILayout.Label("Type", GUILayout.Width(NameSpace));
            _service.DrawLine(TypicalHight);
            GUILayout.Label("Name", GUILayout.Width(NameSpace));
            _service.DrawLine(TypicalHight);

            foreach (var r in _rarities)
            {
                GUILayout.Label(r.ScriptableObject.name, GUILayout.Width(70));
                _service.DrawLine(TypicalHight);
            }

            EditorGUILayout.EndHorizontal();
        }

        private void ShowDropToggles(BallVolume volume)
        {
            foreach (var rarity in _rarities)
            {
                bool contains = _dropListDict[rarity.ScriptableObject].Contains(volume);
                EditorGUI.BeginChangeCheck();
                bool newVal = GUILayout.Toggle(contains, GUIContent.none, GUILayout.Width(70));

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(_settings.ScriptableObject, "Toggle DropList");

                    if (newVal)
                        _dropListDict[rarity.ScriptableObject].Add(volume);
                    else
                        _dropListDict[rarity.ScriptableObject].Remove(volume);

                    EditorUtility.SetDirty(_settings.ScriptableObject);
                }

                _service.DrawLine();
            }
        }

        private void ShowVolumes<VT>(string title, List<AssetData<VT>> volumes) where VT : BallVolume
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(title, EditorStyles.largeLabel);
            EditorGUILayout.EndHorizontal();

            foreach (var cell in volumes)
            {
                EditorGUILayout.BeginHorizontal(GUI.skin.box);
                _service.DrawSprite(cell.GetPropertyValue<Sprite>("_icon"));
                _service.DrawLine();
                EditorGUILayout.LabelField(cell.Type, GUILayout.MaxWidth(NameSpace));
                _service.DrawLine();
                EditorGUILayout.LabelField(cell.GetPropertyValue<string>("_name"), GUILayout.MaxWidth(NameSpace));
                _service.DrawLine();
                ShowDropToggles(cell.ScriptableObject);

                if (GUILayout.Button("Edit"))
                    Selection.activeObject = cell.ScriptableObject;

                EditorGUILayout.EndHorizontal();
            }
        }
    }
}