using System;
using UnityEngine;
using UnityEngine.UI;

public class GameFinishView : MonoBehaviour
{
    [SerializeField] private Button _exit;

    private Action _callback;

    private void OnEnable()
    {
        _exit.AddListener(OnTrigger);
    }

    private void OnDisable()
    {
        _exit.RemoveListener(OnTrigger);
    }

    public void Show(Action callback)
    {
        gameObject.SetActive(true);
        _callback = callback;
    }

    private void OnTrigger()
    {
        _callback.Invoke();
    }
}
