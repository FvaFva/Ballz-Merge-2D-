using System.Collections;
using TMPro;
using UnityEngine;

public class GameRuleView : MonoBehaviour
{
    private float Padding = 20f;
    private RectTransform _rect;

    [SerializeField] private TMP_Text _header;

    void OnEnable()
    {
        StartCoroutine(UpdateSize());
    }

    public GameRuleView Init(string hunt)
    {
        _header.text = hunt;
        _rect = (RectTransform)transform;

        return this;
    }

    private IEnumerator UpdateSize()
    {
        yield return new WaitForEndOfFrame();

        _header.ForceMeshUpdate();
        float huntHeight = _header.preferredHeight;
        _rect.sizeDelta = new Vector2(_rect.sizeDelta.x, huntHeight + Padding);
    }
}
