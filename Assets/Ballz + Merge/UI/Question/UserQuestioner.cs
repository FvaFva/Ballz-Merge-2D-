using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserQuestioner : DependentColorUI
{
    [SerializeField] private TMP_Text _label;
    [SerializeField] private Button _yes;
    [SerializeField] private Button _no;
    [SerializeField] private List<BackgroundUI> _backgroundUIs;

    private Queue<UserQuestion> _questions = new Queue<UserQuestion>();
    private UserQuestion _current;

    private void OnEnable()
    {
        _no.AddListener(OnAnswerNo);
        _yes.AddListener(OnAnswerYes);
    }

    private void OnDisable()
    {
        _no.RemoveListener(OnAnswerNo);
        _yes.RemoveListener(OnAnswerYes);
    }

    public override void ApplyColors(GameColors gameColors)
    {
        foreach (var backgroundUI in _backgroundUIs)
            backgroundUI.ApplyColors(gameColors);
    }

    public void Show(UserQuestion question)
    {
        if (_current.IsEmpty())
        {
            gameObject.SetActive(true);
            Display(question);
        }
        else
        {
            _questions.Enqueue(question);
        }
    }

    public void SetQuestionLabel(UserQuestion question, string label)
    {
        question.Description = label;
        _label.text = question.Description;
    }

    public void SetQuestionFalse(UserQuestion question)
    {
        question.CallBack(false);
        ButtonHandle();
    }


    private void Display(UserQuestion question)
    {
        _current = question;
        _label.text = question.Description;
    }

    private void Hide()
    {
        _current = default;
        _label.text = string.Empty;
        gameObject.SetActive(false);
    }

    private void OnAnswerNo()
    {
        SetQuestionFalse(_current);
    }

    private void OnAnswerYes()
    {
        _current.CallBack(true);
        ButtonHandle();
    }

    private void ButtonHandle()
    {
        if (_questions.TryDequeue(out _current))
            Display(_current);
        else
            Hide();
    }
}