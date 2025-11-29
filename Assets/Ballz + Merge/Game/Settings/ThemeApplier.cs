using System;

public class ThemeApplier : IDisposable
{
    private GameColors _gameColors;
    private SceneSettingData _themeAccentSwitch;

    public ThemeApplier(SceneSettingData themeAccentSwitch, GameColors gameColors)
    {
        _gameColors = gameColors;
        _themeAccentSwitch = themeAccentSwitch;
        _themeAccentSwitch.ValueChanged += SetAccentColors;
    }

    public void Dispose()
    {
        _themeAccentSwitch.ValueChanged -= SetAccentColors;
    }

    private void SetAccentColors(bool isAccent)
    {
        _gameColors.ChangeState(!isAccent);

        if (isAccent)
            PlatformRunner.Run(null, androidAction: () => _gameColors.GetAndroidDynamicAccent());
        else
            _gameColors.ReturnGameColor();
    }
}