using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class UIReorganizer : CyclicBehavior, IDependentScreenOrientation, IDependentSceneSettings
{
    [SerializeField] private SpritesForDifferentUI _sprites;
    [SerializeField] private List<PanelViewProperty> _infoPanels;
    [SerializeField] private List<DependentColorUI> _dependentColorsUI;

    private GameColors _colors;
    private SceneSetting _sceneSetting;

    private void OnEnable()
    {
        if (_sceneSetting != null)
        {
            _sceneSetting.Changed += UpdateSetting;
            UpdateSetting();
        }
    }

    private void OnDisable()
    {
        if (_sceneSetting != null)
            _sceneSetting.Changed -= UpdateSetting;
    }

    public void UpdateScreenOrientation(bool isVertical)
    {
        ChangeAnchors(isVertical);

        if (isVertical)
        {
            ChangeSprite(_sprites.VerticalMainSprite, _sprites.VerticalShaderSprite);
        }
        else
        {
            ChangeSprite(_sprites.HorizontalMainSprite, _sprites.HorizontalShaderSprite);
        }
    }

    public void ConnectToSetting(SceneSetting sceneSetting)
    {
        if (_sceneSetting != null)
            return;

        _sceneSetting = sceneSetting;
        _sceneSetting.Changed += UpdateSetting;
        UpdateSetting();
    }

    public void ConnectUI(DependentColorUI dependentColorUI)
    {
        _dependentColorsUI.Add(dependentColorUI);
    }

    public void ChangeAnchors(bool isVertical)
    {
        foreach (var infoPanelProperty in _infoPanels)
        {
            var (min, max) = isVertical
                ? (infoPanelProperty.VerticalAnchorsMin, infoPanelProperty.VerticalAnchorsMax)
                : (infoPanelProperty.HorizontalAnchorsMin, infoPanelProperty.HorizontalAnchorsMax);

            infoPanelProperty.ChangeAnchors(min, max);
        }
    }

    public void ApplySetting(SceneSetting sceneSetting)
    {
        _colors = sceneSetting.Colors;

        foreach (var dependentColorUI in _dependentColorsUI)
            dependentColorUI.ApplyColors(_colors);
    }

    private void ChangeSprite(Sprite mainSprite, Sprite shaderSprite)
    {
        foreach (var dependentColorUI in _dependentColorsUI)
        {
            if (dependentColorUI is AnimatedButton animatedButton && animatedButton.IsSpriteChangeable)
            {
                animatedButton.ChangeSprite(mainSprite, shaderSprite);
            }
        }
    }

    private void UpdateSetting()
    {
        ApplySetting(_sceneSetting);
    }
}
