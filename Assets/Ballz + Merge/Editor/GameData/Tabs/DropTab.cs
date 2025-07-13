using System;
using System.Collections.Generic;
using System.Linq;
using BallzMerge.Gameplay.Level;
using UnityEditor;
using UnityEngine;

namespace BallzMerge.Editor
{
    public class DropTab : IGameDataTab
    {
        private const int NameSpace = 200;
        private const int IconSpace = 32;

        private List<AssetData<BallVolumeOnHit>> _onHitVolumes;
        private List<AssetData<BallVolumePassive>> _passiveVolumes;
        private List<AssetData<DropRarity>> _rarities;
        private DropList _dropListInput;
        private Dictionary<DropRarity, List<BallVolume>> _dropListDict;
        private Vector2 _scroll;
        private Color _fillColor = Color.magenta;

        public string Title => "Drop";

        public IGameDataTab LoadData()
        {
            _onHitVolumes = new List<AssetData<BallVolumeOnHit>>();
            LoadCellList(_onHitVolumes, "t:BallVolumeOnHit");
            _passiveVolumes = new List<AssetData<BallVolumePassive>>();
            LoadCellList(_passiveVolumes, "t:BallVolumePassive");

            _rarities = new List<AssetData<DropRarity>>();
            LoadCellList(_rarities, "t:DropRarity");
            _rarities = _rarities.OrderBy(r => r.ScriptableObject.Weight).ToList();

            return this;
        }

        public void OnGUI()
        {
            ShowSettings();
            Action<BallVolume> showDropToggles = _dropListInput is null ? (_) => {; } : ShowDropToggles;
            ShowHeader();
            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            ShowVolumes("On hit volumes", _onHitVolumes, showDropToggles);
            ShowVolumes("Passive volumes", _passiveVolumes, showDropToggles);
            EditorGUILayout.EndScrollView();
        }

        private void DrawSprite(Sprite sprite)
        {
            if (sprite != null)
            {
                Texture2D tex = sprite != null ? sprite.texture : null;
                var drawRect = GUILayoutUtility.GetRect(IconSpace, IconSpace, GUILayout.ExpandWidth(false));
                if (tex != null)
                {
                    var uv = sprite.textureRect;
                    uv.x /= tex.width; uv.y /= tex.height;
                    uv.width /= tex.width; uv.height /= tex.height;
                    GUI.DrawTextureWithTexCoords(drawRect, tex, uv, true);
                }
                else
                {
                    EditorGUI.DrawRect(drawRect, Color.gray);
                }
            }
            else
            {
                EditorGUILayout.LabelField("[_icon not found]", GUILayout.Width(IconSpace));
            }
        }

        private void LoadCellList<T>(List<AssetData<T>> list, string filter) where T : ScriptableObject
        {
            var volumesGUIDs = AssetDatabase.FindAssets(filter);

            foreach (var guid in volumesGUIDs)
            {
                var volume = new AssetData<T>(guid);

                if (volume.IsLoaded == false)
                    continue;

                list.Add(volume);
            }
        }

        private void ShowSettings()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

            if (GUILayout.Button("Update", GUILayout.Width(70)))
                LoadData();

            var content = new GUIContent("Drop List Asset", "Chose DropList for edit");
            var temp = (DropList)EditorGUILayout.ObjectField(content, _dropListInput, typeof(DropList), false);

            if (temp != _dropListInput)
            {
                _dropListInput = temp;

                if (_dropListInput is null)
                {
                    _dropListDict = new Dictionary<DropRarity, List<BallVolume>>();
                }
                else
                {
                    _dropListDict = _dropListInput.DropMap;

                    foreach (var rarity in _rarities)
                        if (!_dropListDict.ContainsKey(rarity.ScriptableObject))
                            _dropListDict[rarity.ScriptableObject] = new List<BallVolume>();
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        private void ShowHeader()
        {
            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            GUILayout.Label("Icon", GUILayout.Width(IconSpace));
            DrawLine(10);
            GUILayout.Label("Type", GUILayout.Width(NameSpace));
            DrawLine(10);
            GUILayout.Label("Name", GUILayout.Width(NameSpace));
            DrawLine(10);

            if (_dropListInput is not null)
            {
                foreach (var r in _rarities)
                {
                    GUILayout.Label(r.ScriptableObject.name, GUILayout.Width(70));
                    DrawLine(10);
                }
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
                    Undo.RecordObject(_dropListInput, "Toggle DropList");

                    if (newVal)
                        _dropListDict[rarity.ScriptableObject].Add(volume);
                    else
                        _dropListDict[rarity.ScriptableObject].Remove(volume);

                    EditorUtility.SetDirty(_dropListInput);
                }

                DrawLine();
            }
        }

        private void ShowVolumes<VT>(string title, List<AssetData<VT>> volumes, Action<BallVolume> showDropToggles) where VT : BallVolume
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(title, EditorStyles.largeLabel);
            EditorGUILayout.EndHorizontal();

            foreach (var cell in volumes)
            {
                EditorGUILayout.BeginHorizontal(GUI.skin.box);
                DrawSprite(cell.GetPropertyValue<Sprite>("_icon"));
                DrawLine();
                EditorGUILayout.LabelField(cell.Type, GUILayout.MaxWidth(NameSpace));
                DrawLine();
                EditorGUILayout.LabelField(cell.GetPropertyValue<string>("_name"), GUILayout.MaxWidth(NameSpace));
                DrawLine();
                showDropToggles(cell.ScriptableObject);

                if (GUILayout.Button("Edit"))
                    Selection.activeObject = cell.ScriptableObject;

                EditorGUILayout.EndHorizontal();
            }
        }

        private void DrawLine(int height = IconSpace)
        {
            var last = GUILayoutUtility.GetLastRect();
            var lineRect = new Rect(last.xMax + 2, last.y, 2, height + 10);
            EditorGUI.DrawRect(lineRect, _fillColor);
            GUILayout.Space(6);
        }
    }
}