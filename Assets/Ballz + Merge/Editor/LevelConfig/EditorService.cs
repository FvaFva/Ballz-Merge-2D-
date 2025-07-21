using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BallzMerge.Editor
{
    public class EditorService
    {
        private Color _fillColor = Color.magenta;

        public readonly int IconSpace = 32;

        public AssetData<T> LoadAsset<T>() where T : ScriptableObject
        {
            var guid = AssetDatabase.FindAssets($"t:{typeof(T)}").FirstOrDefault();
            return new AssetData<T>(guid);
        }

        public void DrawSprite(Sprite sprite)
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

        public void DrawLine(float height = 42, float width = 2, int padding = 2, float spacing = 6)
        {
            var last = GUILayoutUtility.GetLastRect();
            var lineRect = new Rect(last.xMax + padding, last.y, width, height);
            EditorGUI.DrawRect(lineRect, _fillColor);
            GUILayout.Space(spacing);
        }

        public void ShowAsset<T>(T asset) where T : ScriptableObject
        {
            EditorGUILayout.ObjectField(asset, typeof(T), false);
        }

        public List<AssetData<T>> LoadObjects<T>() where T : ScriptableObject
        {
            var volumesGUIDs = AssetDatabase.FindAssets($"t:{typeof(T)}");
            var list = new List<AssetData<T>>();

            foreach (var guid in volumesGUIDs)
            {
                var volume = new AssetData<T>(guid);

                if (volume.IsLoaded == false)
                    continue;

                list.Add(volume);
            }

            return list;
        }

        public AssetData<T> CreateAsset<T>(string name, string path, T reference = null) where T : ScriptableObject
        {
            if (string.IsNullOrEmpty(path))
                return null;

            T asset;

            if (reference is null)
                asset = ScriptableObject.CreateInstance<T>();
            else
                asset = Object.Instantiate(reference);

            AssetDatabase.CreateAsset(asset, $"{path}/{name}.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Selection.activeObject = asset;
            EditorGUIUtility.PingObject(asset);

            return new AssetData<T>(asset);
        }
    }
}