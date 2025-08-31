using System.Collections.Generic;
using UnityEngine;

public class GameRulesView : CyclicBehavior, IInfoPanelView, IInitializable
{
    [SerializeField] private GameRuleView _prefab;
    [SerializeField] private GameRulesList _rules;
    [SerializeField] private RectTransform _content;

    private List<GameRuleView> _views;
    private RectTransform _transform;
    private RectTransform _oldParent;

    public void Hide()
    {
        gameObject.SetActive(false);
        _transform.SetParent(_oldParent);
    }

    public void Init()
    {
        _views = new List<GameRuleView>();

        foreach (string hint in _rules.Rules)
            _views.Add(Instantiate(_prefab, _content).Init(hint));

        _transform = (RectTransform)transform;
        _oldParent = (RectTransform)_transform.parent;
    }

    public void Show(RectTransform showcase)
    {
        _transform.SetParent(showcase);
        gameObject.SetActive(true);
    }
}
