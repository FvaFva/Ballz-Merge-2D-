using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectionView : MonoBehaviour
{
    [SerializeField] private Button _trigger;
    [SerializeField] private TMP_Text _header;
    [SerializeField] private GameObject _completed;

    private LevelSettings _data;

    public event Action<LevelSettings> Selected;

    private void OnEnable()
    {
        _trigger.AddListener(OnTriggered);
    }

    public LevelSelectionView Show(LevelSettings level, bool completed)
    {
        _data = level;
        _header.text = _data.Title;
        _completed.SetActive(completed);
        return this;
    }

    private void OnTriggered()
    {
        Selected?.Invoke(_data);
    }
}
