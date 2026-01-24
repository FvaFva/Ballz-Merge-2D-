using System;
using TMPro;
using UnityEngine;

public class LevelSelectionView : MonoBehaviour
{
    [SerializeField] private ButtonProperty _trigger;
    [SerializeField] private TMP_Text _header;
    [SerializeField] private GameObject _completed;

    public LevelSettings Data { get; private set; }

    public event Action<LevelSettings> Selected;

    private void OnEnable()
    {
        _trigger.ChangeListeningState(OnTriggered, true);
    }

    private void OnDisable()
    {
        _trigger.ChangeListeningState(OnTriggered, false);
    }

    public LevelSelectionView Init(LevelSettings level)
    {
        Data = level;
        _header.text = Data.Title;
        return this;
    }

    public void ApplyColors(GameColors gameColors)
    {
        _trigger.ApplyColors(gameColors);
    }

    public void SetState(bool state)
    {
        gameObject.SetActive(state);
    }

    public void ShowStatus(bool completed)
    {
        _completed.SetActive(completed);
    }

    private void OnTriggered()
    {
        Selected?.Invoke(Data);
    }
}
