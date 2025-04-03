using BallzMerge.Gameplay.Level;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(MoveSettingsRange))]
public class MoveSettingsRangeDrawer : PropertyDrawer
{
    private const string InitialWave = "InitialWave";
    private const string TerminalWave = "TerminalWave";

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        SerializedProperty initialWave = property.FindPropertyRelative(InitialWave);
        SerializedProperty terminalWave = property.FindPropertyRelative(TerminalWave);

        float dashWidth = position.width / 10;
        float fieldWidth = dashWidth * 3;
        float firstLineHeight = EditorGUI.GetPropertyHeight(initialWave);

        Rect labelRect = new Rect(position.x, position.y, fieldWidth, firstLineHeight);
        Rect initialWaveRect = new Rect(position.x + fieldWidth, position.y, fieldWidth - dashWidth / 2, firstLineHeight);
        Rect dashRect = new Rect(position.x + fieldWidth * 2, position.y, dashWidth, firstLineHeight);
        Rect terminalWaveRect = new Rect(position.x + fieldWidth * 2 + dashWidth, position.y, fieldWidth - dashWidth, firstLineHeight);

        EditorGUI.LabelField(labelRect, "Waves:");
        EditorGUI.PropertyField(initialWaveRect, initialWave, GUIContent.none);
        EditorGUI.LabelField(dashRect, "-");
        EditorGUI.PropertyField(terminalWaveRect, terminalWave, GUIContent.none);

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property.FindPropertyRelative(InitialWave));
    }
}