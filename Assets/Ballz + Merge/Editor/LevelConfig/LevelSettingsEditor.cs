using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BallzMerge.Editor
{
    public class LevelSettingsEditor : EditorWindow
    {
        private EditorService _service = new EditorService();
        private int _selectedTab;
        private List<IGameSettingsTab> _tabs;
        private AllSettingsViewer _settings = new AllSettingsViewer();
        private bool _picking;

        [MenuItem("Tools/Game/Level configurator")]
        public static void ShowWindow() => GetWindow<LevelSettingsEditor>("Level configurator");

        private void OnEnable()
        {
            _settings.LoadData();

            _tabs = new List<IGameSettingsTab>
            {
                new DropTab().LoadData(_settings.Current),
                new LevelTab().LoadData(_settings.Current)
            };

            _settings.Changed += RebuildSettings;
        }

        void OnDisable()
        {
            _settings.Changed -= RebuildSettings;
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            {
                _settings.OnGUI();
                EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                {
                    _service.ShowAsset(_settings.Current.ScriptableObject);
                    _settings.Current.SerializedObject.Update();
                    EditorGUILayout.Space();

                    EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                    {
                        EditorGUILayout.PropertyField(_settings.Current.GetProperty("_title"));

                        if (GUILayout.Button("Copy settings"))
                        {
                            _picking = true;
                            EditorGUIUtility.ShowObjectPicker<LevelSettings>(null, false, "", 0);
                        }

                        if (_picking && Event.current.commandName == "ObjectSelectorClosed")
                        {
                            var picked = EditorGUIUtility.GetObjectPickerObject() as LevelSettings;
                            _picking = false;

                            if (picked != null)
                            {
                                _settings.CopyFrom(picked);
                                GUI.FocusControl(null);
                                Repaint(); 
                            }
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space();
                    DrawToolbar();
                    EditorGUILayout.Space();
                    _tabs[_selectedTab].OnGUI();
                    _settings.Current.SerializedObject.ApplyModifiedProperties();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawToolbar()
        {
            var titles = _tabs.Select(t => t.Title).ToArray();
            _selectedTab = GUILayout.Toolbar(_selectedTab, titles, GUILayout.Height(24));
        }

        private void RebuildSettings()
        {
            foreach (var tab in _tabs)
                tab.LoadData(_settings.Current);
        }
    }
}
