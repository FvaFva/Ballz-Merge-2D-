using UnityEngine;
using TMPro;
using System;

public class LevelDifficultView : MonoBehaviour
{
    [SerializeField] private ButtonProperty _triggerButton;
    [SerializeField] private TMP_Text _difficultText;
    [SerializeField] private ToggleView _toggleView;

    public LevelDifficult LevelDifficult { get; private set; }

    public event Action<LevelDifficultView> Selected;

    private void OnEnable()
    {
        _triggerButton.ChangeListeningState(SelectDifficult, true);
    }

    private void OnDisable()
    {
        _triggerButton.ChangeListeningState(SelectDifficult, false);
    }

    public LevelDifficultView Init(LevelDifficult levelDifficult)
    {
        LevelDifficult = levelDifficult;
        _difficultText.text = LevelDifficult.ToString();
        _toggleView.Initialize();
        return this;
    }

    public void ApplyColors(GameColors gameColors)
    {
        _triggerButton.ApplyColors(gameColors);
        _toggleView.ApplyColors(gameColors);
    }

    private void SelectDifficult()
    {
        Selected?.Invoke(this);
    }

    public void Select()
    {
        _toggleView.Select();
    }

    public void Unselect()
    {
        _toggleView.Unselect();
    }
}