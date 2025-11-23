using BallzMerge.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayApplier : IDisposable
{
    private const float ApplyDelay = 0.2f;
    private const float ApplyTimeout = 15f;

    private Button _applyButton;
    private Resolution _resolution;
    private ScreenMode _screenMode;
    private IGameSettingData _resolutionData;
    private IGameSettingData _displayModeData;
    private DisplayResolution _displayResolution;

    private bool _isResolutionLoaded;
    private bool _isScreenModeLoaded;
    private int _lastWidth;
    private int _lastHeight;
    private ScreenMode _lastScreenMode;
    private Coroutine _applyCoroutine;
    private bool _isResolutionApplied;
    private bool _isLoad;
    private Dictionary<ScreenMode, FullScreenMode> _displayRelation;

    private UserQuestioner _userQuestioner;

    public event Action<IGameSettingData> Applied;
    public event Action<IGameSettingData> Discarded;

    public DisplayApplier(Button applyButton, UserQuestioner questioner)
    {
        _applyButton = applyButton;
        _userQuestioner = questioner;
        _applyButton.onClick.AddListener(ApplyResolution);

        _displayRelation = new Dictionary<ScreenMode, FullScreenMode>
        {
            { ScreenMode.Fullscreen, FullScreenMode.ExclusiveFullScreen },
            { ScreenMode.Windowed, FullScreenMode.Windowed },
            { ScreenMode.WindowedFullscreen, FullScreenMode.FullScreenWindow },
            { ScreenMode.Resizable, FullScreenMode.Windowed }
        };
    }

    public void Dispose()
    {
        _applyButton.onClick.RemoveListener(ApplyResolution);
    }

    public void SetResolution(Resolution resolution, IGameSettingData settingData, DisplayResolution displayResolution)
    {
        _resolution = resolution;
        _resolutionData = settingData;
        _displayResolution = displayResolution;
    }

    public void LoadResolution()
    {
        if (_isResolutionLoaded == true)
            return;

        _isResolutionLoaded = true;
        LoadResolutionAndScreenMode();
    }

    public void SetScreenMode(ScreenMode screenMode, IGameSettingData settingData)
    {
        _screenMode = screenMode;
        _displayModeData = settingData;
    }

    public void LoadScreenMode()
    {
        if (_isScreenModeLoaded == true)
            return;

        _isScreenModeLoaded = true;
        LoadResolutionAndScreenMode();
    }

    private void LoadResolutionAndScreenMode()
    {
        if (_isResolutionLoaded == true && _isScreenModeLoaded == true)
        {
            _isLoad = true;
            ApplyResolution();
        }
    }

    private void ApplyResolution()
    {
        if (_lastWidth == _resolution.width && _lastHeight == _resolution.height && _lastScreenMode == _screenMode)
            return;

        SetResolution(AfterResolutionApplied);
    }

    private void AfterResolutionApplied()
    {
        if (_isResolutionApplied == false)
            return;

        _displayResolution.ChangeState(!(_screenMode == ScreenMode.Resizable));
    }

    private void SetResizable(ScreenMode screenMode, ScreenMode comparedScreenMode)
    {
        if (screenMode == comparedScreenMode)
            return;

        CoroutineRunner.Instance.StartCoroutine(SetResizeNextFrame(screenMode == ScreenMode.Resizable));
    }

    private void SetResolution(Action actionTimeOut)
    {
        Screen.SetResolution(_resolution.width, _resolution.height, _displayRelation[_screenMode]);
        SetResizable(_screenMode, _lastScreenMode);

        if (_isLoad)
        {
            ApplyResolutionQuestion(true, actionTimeOut);
            _isLoad = false;
            return;
        }

        UserQuestion applyResolutionQuestion = new UserQuestion((bool answer) => ApplyResolutionQuestion(answer, actionTimeOut), $"Apply new resolution?\nResolution will be rolled back in {ApplyDelay} seconds...");
        _userQuestioner.Show(applyResolutionQuestion);
        _applyCoroutine = CoroutineRunner.Instance.StartCoroutine(ApplyResolutionTime(applyResolutionQuestion));
    }

    private void ApplyResolutionQuestion(bool answer, Action actionTimeOut)
    {
        if (_applyCoroutine != null)
        {
            CoroutineRunner.Instance.StopCoroutine(_applyCoroutine);
            _applyCoroutine = null;
        }

        if (answer)
        {
            _isResolutionApplied = true;
            _lastWidth = _resolution.width;
            _lastHeight = _resolution.height;
            _lastScreenMode = _screenMode;
            Applied?.Invoke(_displayModeData);
            Applied?.Invoke(_resolutionData);
        }
        else
        {
            DiscardResolution();
        }

        actionTimeOut();
    }

    private IEnumerator ApplyResolutionTime(UserQuestion applyResolutionQuestion)
    {
        float timer = 0f;

        while (timer < ApplyTimeout)
        {
            timer += Time.unscaledDeltaTime;
            _userQuestioner.SetQuestionLabel(applyResolutionQuestion, $"Apply new resolution?\nResolution will be rolled back in {Mathf.CeilToInt(ApplyTimeout - timer)} seconds...");
            yield return null;
        }

        _userQuestioner.SetQuestionFalse(applyResolutionQuestion);
    }

    private void DiscardResolution()
    {
        _isResolutionApplied = false;
        Screen.SetResolution(_lastWidth, _lastHeight, _displayRelation[_lastScreenMode]);
        SetResizable(_lastScreenMode, _screenMode);
        Discarded?.Invoke(_displayModeData);
        Discarded?.Invoke(_resolutionData);
    }

    private IEnumerator SetResizeNextFrame(bool state)
    {
        yield return new WaitForSeconds(ApplyDelay);

        PlatformRunner.Run(null,
        windowsAction: () =>
        {
            WindowResizerWindows.SetResizable(state);
        },
        macAction: () =>
        {
            WindowResizerMac.SetResizable(state);
        });
    }
}