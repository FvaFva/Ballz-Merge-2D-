using UnityEngine;
using UnityEngine.UI;

public class RollupPanel : MonoBehaviour
{
    [SerializeField] private Button _close;
    [SerializeField] private Button _open;
    [SerializeField] private RectTransform _content;

    private void Awake()
    {
        ChangeActivity(false);
    }

    private void OnEnable()
    {
        _close.AddListener(OnClose);
        _open.AddListener(OnOpen);
    }

    private void OnDisable()
    {
        _close.RemoveListener(OnClose);
        _open.RemoveListener(OnOpen);
    }

    private void OnOpen()
    {
        ChangeActivity(true);

    }

    private void OnClose()
    {
        ChangeActivity(false);
    }

    private void ChangeActivity(bool isActive)
    {
        _open.gameObject.SetActive(!isActive);
        _close.gameObject.SetActive(isActive);
        _content.gameObject.SetActive(isActive);
    }
}
