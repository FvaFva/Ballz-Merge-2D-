using System;
using UnityEngine;
using UnityEngine.UI;

public class BlockerToClick : MonoBehaviour
{
    [SerializeField] private Button _trigger;

    public event Action Clicked;

    private void OnEnable() => _trigger.AddListener(Triggered);

    private void OnDisable() => _trigger.RemoveListener(Triggered);

    public void Activate() => gameObject.SetActive(true);

    private void Triggered()
    {
        Clicked?.Invoke();
        gameObject.SetActive(false);
    }
}
