using System.Collections;
using TMPro;
using UnityEngine;

public class GameRuleView : DependentColorUI
{
    private float Padding = 20f;
    private RectTransform _rect;

    [SerializeField] private TMP_Text _header;
    [SerializeField] private BackgroundUI _backgroundUI;

    private void OnEnable()
    {
        StartCoroutine(UpdateSize());
    }

    public GameRuleView Init(string hunt)
    {
        _header.text = hunt;
        _rect = (RectTransform)transform;

        return this;
    }

    public override void ApplyColors(GameColors gameColors)
    {
        _backgroundUI.ApplyColors(gameColors);
    }

    private IEnumerator UpdateSize()
    {
        yield return new WaitForEndOfFrame();

        _header.ForceMeshUpdate();
        float huntHeight = _header.preferredHeight;
        _rect.sizeDelta = new Vector2(_rect.sizeDelta.x, huntHeight + Padding);
    }
}
