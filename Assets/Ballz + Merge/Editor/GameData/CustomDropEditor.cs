using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BallzMerge.Editor
{
    public class GameDataEditor : EditorWindow
    {
        private int _selectedTab;
        private List<IGameDataTab> _tabs;

        [MenuItem("Tools/Game/Data Editor")]
        public static void ShowWindow()
        {
            GetWindow<GameDataEditor>("Game Data");
        }

        private void OnEnable()
        {
            _tabs = new List<IGameDataTab>
            {
                new DropTab().LoadData(),
                new LevelTab().LoadData()
            };
        }

        private void OnGUI()
        {
            DrawToolbar();
            EditorGUILayout.Space();
            _tabs[_selectedTab].OnGUI();
        }

        private void DrawToolbar()
        {
            var titles = _tabs.Select(t => t.Title).ToArray();
            _selectedTab = GUILayout.Toolbar(_selectedTab, titles, GUILayout.Height(24));
        }
    }
}
