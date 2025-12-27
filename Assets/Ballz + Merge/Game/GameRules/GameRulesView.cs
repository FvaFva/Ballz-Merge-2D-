using BallzMerge.Root;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameRulesView : DependentColorUI, IInfoPanelView, IDependentScreenOrientation
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
    private InfoPanelShowcase _showcase;

    public void Update()
    {
        _fitter.aspectRatio = _imageRect.sizeDelta.x / _imageRect.sizeDelta.y;
    }

    public void Init(InfoPanelShowcase showcase)
    {
        _transform = (RectTransform)transform;
        _oldParent = (RectTransform)_transform.parent;
        _previousButton.AddListener(OnClickPrevious);
        _nextButton.AddListener(OnClickNext);
        _showcase = showcase;
    }

    public void ShowRule(int index = -1)
    {
        if (index == -1)
            index = Random.Range(0, _rules.Rules.Count);
        else
            index = Mathf.Clamp(index, 0, _rules.Rules.Count - 1);

        _currentRuleIndex = index;
        ApplyRule(_rules.Rules[_currentRuleIndex]);
        _showcase.Show(this);
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
        if (_currentRuleIndex <= 0)
            return;

        ApplyRule(_rules.Rules[--_currentRuleIndex]);
    }

    private void OnClickNext()
    {
        if (_currentRuleIndex >= _rules.Rules.Count - 1)
            return;

        ApplyRule(_rules.Rules[++_currentRuleIndex]);
    }

    private void ApplyRule(GameRule rule)
    {
        _title.text = rule.Label;
        _imageView.sprite = rule.Reference;
        _description.text = rule.Description;
        _counter.text = $"{_currentRuleIndex + 1} / {_rules.Rules.Count}";
    }
}
