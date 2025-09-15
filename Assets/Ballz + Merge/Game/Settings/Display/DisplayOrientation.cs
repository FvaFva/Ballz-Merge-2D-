using BallzMerge.Data;
using System.Collections.Generic;
using UnityEngine;

public class DisplayOrientation : IGameSettingData
{
    public string Name { get; private set; }
    public float Value { get; private set; }
    public string Label { get; private set; }
    public int? CountOfPresets { get; private set; }

    private List<DisplayOrientationProperty> _orientations;
    private ScreenOrientation _orientation;

    public DisplayOrientation(string name)
    {
        Name = name;

        _orientations = new List<DisplayOrientationProperty>
        {
            { new DisplayOrientationProperty(ScreenOrientation.AutoRotation, "Auto") },
            { new DisplayOrientationProperty(ScreenOrientation.Portrait, "Portrait") },
            { new DisplayOrientationProperty(ScreenOrientation.LandscapeLeft, "Landscape Left") },
            { new DisplayOrientationProperty(ScreenOrientation.LandscapeRight, "Landscape Right") },
        };

        CountOfPresets = _orientations.Count - 1;

        for (int i = 0; i < _orientations.Count; i++)
        {
            if (_orientations[i].Orientation == Screen.orientation)
            {
                Value = i;
                break;
            }
        }

        _orientation = _orientations[Mathf.RoundToInt(Value)].Orientation;
    }

    public void Get(float value)
    {
        Value = CountOfPresets < value ? (float)CountOfPresets : value;
        Change(Value);
    }

    public void Change(float value)
    {
        Value = value;
        Label = _orientations[Mathf.RoundToInt(Value)].OrientationName;
        _orientation = _orientations[Mathf.RoundToInt(Value)].Orientation;
        Screen.orientation = _orientation;
    }
}
