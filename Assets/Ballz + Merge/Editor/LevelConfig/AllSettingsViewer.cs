using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BallzMerge.Editor
{
    public class AllSettingsViewer
    {
        private const string LEVELS_STORAGE = "Assets/Ballz + Merge/Gameplay/Level/Settings/Levels";
        private const float BUTTON_WIDTH = 150;
        private const float TOGGLE_WIDTH = 45;

        private AssetData<LevelSettings> _current;
        private EditorService _service = new EditorService();
        private bool _isShow = true;
        private Vector2 _scroll;
        private List<AssetData<LevelSettings>> _allSettings;
        private AssetData<LevelSettingsMap> _map;
        private SerializedProperty _available;

        public AssetData<LevelSettings> Current => _current;
        public event Action Changed;

        public string Title => "All levels";

        public void LoadData()
        {
            _allSettings = _service.LoadObjects<LevelSettings>().OrderBy(level => level.ScriptableObject.Title).ToList();
            _current = _service.LoadAsset<LevelSettings>();
            _map = _service.LoadAsset<LevelSettingsMap>();
            _available = _map.GetProperty("_available");
            RebuildMap();
        }

        public void OnGUI()
        {
            if (_isShow)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(200), GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(false));
                {
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Add level", GUILayout.ExpandWidth(true)))
                        CreateLevelSetting();
                    EditorGUILayout.Space(4);
                    if (GUILayout.Button("Copy level", GUILayout.ExpandWidth(true)))
                        CreateLevelSetting(_current.ScriptableObject);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space(4);

                    EditorGUILayout.BeginHorizontal(GUI.skin.box);
                    EditorGUILayout.LabelField("Levels", EditorStyles.boldLabel, GUILayout.Width(BUTTON_WIDTH));
                    _service.DrawLine(EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
                    EditorGUILayout.LabelField("Available", EditorStyles.boldLabel, GUILayout.Width(TOGGLE_WIDTH));
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space(4);

                    _scroll = EditorGUILayout.BeginScrollView(_scroll);
                    {
                        foreach (var setting in _allSettings)
                        {
                            EditorGUILayout.BeginHorizontal(GUI.skin.box);
                            {
                                ShowButton(setting);
                                _service.DrawLine(EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
                                ShowToggle(setting);
                            }
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.Space(1);
                        }
                    }
                    EditorGUILayout.EndScrollView();
                }
                EditorGUILayout.EndVertical();
            }

            if (GUILayout.Button(_isShow ? "<" : ">", GUILayout.Width(20), GUILayout.ExpandHeight(true)))
                _isShow = !_isShow;
        }

        public void CopyFrom(LevelSettings source)
        {
            if (source is null)
                return;
            
            Undo.RecordObject(_current.ScriptableObject, "Copy LevelSettings");
            _current.ScriptableObject.Copy(new BlocksSettings(source.BlocksSettings), new DropSettings(source.DropSettings));
            EditorUtility.SetDirty(_current.ScriptableObject);
            AssetDatabase.SaveAssets();
        }

        private void ShowToggle(AssetData<LevelSettings> setting)
        {
            int mapIndex = GetIndexInMap(setting.ScriptableObject);
            bool oldValue = mapIndex != -1;
            EditorGUI.BeginChangeCheck();
            bool newVal = GUILayout.Toggle(oldValue, GUIContent.none, GUILayout.Width(TOGGLE_WIDTH));

            if (EditorGUI.EndChangeCheck())
            {
                if (newVal)
                    InsertInProperty(_available, setting.ScriptableObject);
                else
                    _available.DeleteArrayElementAtIndex(mapIndex);

                _map.SerializedObject.ApplyModifiedProperties();
            }
        }

        private void InsertInProperty(SerializedProperty property, LevelSettings setting)
        {
            var size = property.arraySize;
            property.InsertArrayElementAtIndex(size);
            property.GetArrayElementAtIndex(size).objectReferenceValue = setting;
        }

        private int GetIndexInMap(LevelSettings level)
        {
            for (int i = 0; i < _available.arraySize; i++)
            {
                if (_available.GetArrayElementAtIndex(i).objectReferenceValue == level)
                    return i;
            }

            return -1;
        }

        private void ShowButton(AssetData<LevelSettings> setting)
        {
            if (GUILayout.Button(setting.ScriptableObject.Title, GUILayout.Width(BUTTON_WIDTH)))
            {
                _current = setting;
                Changed?.Invoke();
            }
        }

        private void CreateLevelSetting(LevelSettings reference = null)
        {
            string name = _allSettings.Count.ToString("D4");
            _current = _service.CreateAsset($"Level_{name}", LEVELS_STORAGE, reference);
            _allSettings.Add(_current);
            _allSettings = _allSettings.OrderBy(level => level.ScriptableObject.Title).ToList();
            _current.SerializedObject.Update();
            var title = _current.GetProperty("_title");
            var guid = _current.GetProperty("_iD");
            title.stringValue = name;
            guid.intValue = _map.ScriptableObject.GetID();
            Undo.RecordObject(_current.ScriptableObject, "Change Title");
            _current.SerializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(_current.ScriptableObject);
            Changed?.Invoke();
            RebuildMap();
        }

        private void RebuildMap()
        {
            Undo.RecordObject(_map.ScriptableObject, "Change map");
            var allInMap = _map.GetProperty("_all");
            allInMap.ClearArray();

            foreach (var setting in _allSettings.Select(s => s.ScriptableObject))
                InsertInProperty(allInMap, setting);

            EditorUtility.SetDirty(_map.ScriptableObject);
            _map.SerializedObject.ApplyModifiedProperties();
        }
    }
}