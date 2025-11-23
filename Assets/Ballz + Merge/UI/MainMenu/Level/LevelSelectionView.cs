using System;
using TMPro;
using UnityEngine;

public class LevelSelectionView : MonoBehaviour
{
    [SerializeField] private ButtonProperty _trigger;
    [SerializeField] private TMP_Text _header;
    [SerializeField] private GameObject _completed;

    private LevelSettings _data;

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
        _data = level;
        _header.text = _data.Title;
        return this;
    }

    public void ApplyColors(GameColors gameColors)
    {
        _trigger.ApplyColors(gameColors);
    }

    public void ShowStatus(bool completed)
    {
        _completed.SetActive(completed);
    }

    private void OnTriggered()
    {
        Selected?.Invoke(_data);
    }
}
