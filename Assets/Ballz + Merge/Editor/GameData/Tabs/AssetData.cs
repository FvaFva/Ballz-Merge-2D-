using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BallzMerge.Editor
{
    public class AssetData<T> where T : ScriptableObject
    {
        private readonly Dictionary<string, SerializedProperty> _cashedProperty;

        public readonly string Type;
        public readonly T ScriptableObject;
        public readonly SerializedObject SerializedObject;
        public readonly bool IsLoaded;

        public AssetData(T so)
        {
            ScriptableObject = so;
            IsLoaded = ScriptableObject != null;

            if (!IsLoaded)
                return;

            _cashedProperty = new Dictionary<string, SerializedProperty>();
            SerializedObject = new SerializedObject(ScriptableObject);
            SerializedObject.Update();
            Type = ScriptableObject.GetType().ToString();
        }

        public AssetData(string guid)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            ScriptableObject = AssetDatabase.LoadAssetAtPath<T>(path);
            IsLoaded = ScriptableObject != null;

            if (!IsLoaded)
                return;

            _cashedProperty = new Dictionary<string, SerializedProperty>();
            SerializedObject = new SerializedObject(ScriptableObject);
            SerializedObject.Update();
            Type = ScriptableObject.GetType().ToString();
        }

        public SerializedProperty GetProperty(string name)
        {
            if (_cashedProperty.ContainsKey(name) == false)
            {
                var prop = SerializedObject.FindProperty(name);
                _cashedProperty.Add(name, prop);
                return prop;
            }

            return _cashedProperty[name];
        }

        public PropType GetPropertyValue<PropType>(string name)
        {
            var prop = GetProperty(name);

            if (prop is null)
                return default;
            else
                return (PropType)GetValue(prop);
        }

        private object GetValue(SerializedProperty prop)
        {
            switch (prop.propertyType)
            {
                case SerializedPropertyType.Integer:
                    return prop.intValue;
                case SerializedPropertyType.Boolean:
                    return prop.boolValue;
                case SerializedPropertyType.Float:
                    return prop.floatValue;
                case SerializedPropertyType.String:
                    return prop.stringValue;
                case SerializedPropertyType.Color:
                    return prop.colorValue;
                case SerializedPropertyType.ObjectReference:
                    return prop.objectReferenceValue;
                case SerializedPropertyType.Enum:
                    return prop.enumValueIndex;
                case SerializedPropertyType.Vector2:
                    return prop.vector2Value;
                case SerializedPropertyType.Vector3:
                    return prop.vector3Value;
                case SerializedPropertyType.Vector4:
                    return prop.vector4Value;
                case SerializedPropertyType.Rect:
                    return prop.rectValue;
                case SerializedPropertyType.AnimationCurve:
                    return prop.animationCurveValue;
                case SerializedPropertyType.Bounds:
                    return prop.boundsValue;
                case SerializedPropertyType.Gradient:
                    return prop.gradientValue;
                case SerializedPropertyType.Generic:
                    if (prop.isArray)
                    {
                        var list = new List<object>();
                        for (int i = 0; i < prop.arraySize; i++)
                        {
                            var elem = prop.GetArrayElementAtIndex(i);
                            list.Add(GetValue(elem));
                        }
                        return list;
                    }
                    {
                        var dict = new Dictionary<string, object>();
                        var iterator = prop.Copy();
                        var end = prop.GetEndProperty();
                        iterator.NextVisible(true);
                        while (!SerializedProperty.EqualContents(iterator, end))
                        {
                            dict[iterator.name] = GetValue(iterator);
                            iterator.NextVisible(false);
                        }
                        return dict;
                    }
                default:
                    return null;
            }
        }
    }
}