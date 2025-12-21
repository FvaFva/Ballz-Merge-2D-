using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameRulesView : DependentColorUI, IInfoPanelView, IInitializable, IDependentScreenOrientation
{
    [SerializeField] private AdaptiveLayoutGroupStretching _group;
    [SerializeField] private AspectRatioFitter _fitter;
    [SerializeField] private RectTransform _imageRect;
    [SerializeField] private Image _imageView;
    [SerializeField] private TMP_Text _title;
    [SerializeField] private TMP_Text _description;
    [SerializeField] private GameRulesList _rules;
    [SerializeField] private Button _previousButton;
    [SerializeField] private Button _nextButton;
    [SerializeField] private TMP_Text _counter;
    [SerializeField] private List<DependentColorUI> _backgroundUIs;

    private RectTransform _transform;
    private RectTransform _oldParent;
    private int _currentRuleIndex;

    public void Update()
    {
        _fitter.aspectRatio = _imageRect.sizeDelta.x / _imageRect.sizeDelta.y;
    }

    public void Init()
    {
        _transform = (RectTransform)transform;
        _oldParent = (RectTransform)_transform.parent;
        _previousButton.AddListener(OnClickPrevious);
        _nextButton.AddListener(OnClickNext);
        _currentRuleIndex = 1;
        ApplyRule(_rules.Rules[0]);
    }

    public override void ApplyColors(GameColors gameColors)
    {
        foreach (var backgroundUI in _backgroundUIs)
            backgroundUI.ApplyColors(gameColors);
    }

    public void UpdateScreenOrientation(bool isVertical)
    {
        _group.UpdateScreenOrientation(isVertical);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        _transform.SetParent(_oldParent);
    }

    public void Show(RectTransform showcase)
    {
        _transform.SetParent(showcase);
        gameObject.SetActive(true);
    }

    private void OnClickPrevious()
    {
        if (_currentRuleIndex <= 1)
            return;

        ApplyRule(_rules.Rules[--_currentRuleIndex - 1]);
    }

    private void OnClickNext()
    {
        if (_currentRuleIndex >= _rules.Rules.Count)
            return;

        ApplyRule(_rules.Rules[++_currentRuleIndex - 1]);
    }

    private void ApplyRule(GameRule rule)
    {
        _title.text = rule.Label;
        _imageView.sprite = rule.Reference;
        _description.text = rule.Description;
        _counter.text = $"{_currentRuleIndex} / {_rules.Rules.Count}";
    }
}
