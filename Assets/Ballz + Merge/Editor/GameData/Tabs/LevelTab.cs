using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BallzMerge.Editor
{
    public class LevelTab : IGameDataTab
    {
        private AssetData<MoveSettingsCountBlocks> _settings;
        private Vector2 _scroll;
        private SerializedProperty _waves;
        private float _rowH;

        public string Title => "Level";

        public IGameDataTab LoadData()
        {
            _settings = LoadAsset<MoveSettingsCountBlocks>();
            _waves = _settings.GetProperty("_properties");
            _rowH = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            return this;
        }

        public void OnGUI()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Settings: ", EditorStyles.largeLabel, GUILayout.MaxWidth(70));
            ShowAsset(_settings.ScriptableObject);
            EditorGUILayout.EndHorizontal();


            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            _settings.SerializedObject.Update();
            DrawWaves();
            _settings.SerializedObject.ApplyModifiedProperties();
            EditorGUILayout.EndScrollView();
        }

        private AssetData<T> LoadAsset<T>() where T : ScriptableObject
        {
            var guid = AssetDatabase.FindAssets($"t:{typeof(T)}").FirstOrDefault();
            return new AssetData<T>(guid);
        }

        private void ShowAsset<T>(T asset) where T : ScriptableObject
        {
            EditorGUILayout.ObjectField(asset, typeof(T), false);
        }

        private void DrawWaves()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

            if (GUILayout.Button("Add wave"))
                _waves.arraySize++;

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            for (int i = 0; i < _waves.arraySize; i++)
            {
                if (DrawWave(i) == false)
                    break;
            }
        }

        private bool DrawWave(int i)
        {
            string waveName = $"Wave {i + 1}";
            var wave = _waves.GetArrayElementAtIndex(i);

            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Delete", GUILayout.Width(70)))
                {
                    _waves.DeleteArrayElementAtIndex(i);
                    return false;
                }
                
                wave.isExpanded = EditorGUILayout.Foldout(wave.isExpanded, waveName);
            }
            EditorGUILayout.EndHorizontal();

            if (wave.isExpanded)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                {
                    var count = wave.FindPropertyRelative("Count");
                    var numbers = wave.FindPropertyRelative("Number");
                    int max = Mathf.Max(count.arraySize, numbers.arraySize);
                    DrawColum("Count", count, max > count.arraySize ? max - count.arraySize : 0);
                    GUILayout.Space(5);
                    DrawColum("Number", numbers, max > numbers.arraySize ? max - numbers.arraySize : 0);
                }
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel--;
            }
            return true;
        }

        private void DrawColum(string header, SerializedProperty property, int spacing)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.ExpandWidth(true));
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField($"Settings {header}s:", EditorStyles.largeLabel);

                    if (GUILayout.Button("Add"))
                        property.arraySize++;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField(header, EditorStyles.largeLabel, GUILayout.Width(70));
                    EditorGUILayout.LabelField("Weight", EditorStyles.largeLabel, GUILayout.Width(70));
                    EditorGUILayout.Space(6);
                    EditorGUILayout.LabelField("Delete", EditorStyles.largeLabel, GUILayout.Width(20));
                }
                EditorGUILayout.EndHorizontal();
                
                for (int i = 0; i < property.arraySize; i++)
                {
                    var setting = property.GetArrayElementAtIndex(i);
                    EditorGUILayout.BeginHorizontal(GUI.skin.box);
                    {
                        EditorGUILayout.PropertyField(setting.FindPropertyRelative("Value"), GUIContent.none, GUILayout.Width(70));
                        EditorGUILayout.PropertyField(setting.FindPropertyRelative("Weight"), GUIContent.none, GUILayout.Width(70));

                        if (GUILayout.Button("X", GUILayout.Width(20)))
                        {
                            property.DeleteArrayElementAtIndex(i);
                            break;
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }

                GUILayout.Space(spacing * _rowH);
            }
            EditorGUILayout.EndVertical();
        }
    }
}