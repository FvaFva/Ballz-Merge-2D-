using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserQuestioner : MonoBehaviour
{
    private const float TimeScaleForQuestion = 0;

    [SerializeField] private TMP_Text _label;
    [SerializeField] private Button _yes;
    [SerializeField] private Button _no;

    private Queue<UserQuestion> _questions = new Queue<UserQuestion>();
    private float _lastTimeScale;
    private UserQuestion _current;

    public event Action<UserQuestion> Answer;

    private void Awake()
    {
        _lastTimeScale = Time.timeScale;
        Hide();
    }

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

    public void Show(UserQuestion question)
    {
        if (_current.IsEmpty())
        {
            _lastTimeScale = Time.timeScale;
            Time.timeScale = TimeScaleForQuestion;
            gameObject.SetActive(true);
            Display(question);
        }
        else
        {
            _questions.Enqueue(question);
        }
    }

    private void Display(UserQuestion question)
    {
        _current = question;
        _label.text = question.Description;
    }

    private void ButtonHandle()
    {
        Answer?.Invoke(_current);

        if (_questions.TryDequeue(out _current))
            Display(_current);
        else
            Hide();
    }

    private void Hide()
    {
        _current = default;
        Time.timeScale = _lastTimeScale;
        _label.text = String.Empty;
        gameObject.SetActive(false);
    }

    private void OnAnswerNo()
    {
        _current.IsPositiveAnswer = false;
        ButtonHandle();
    }

    private void OnAnswerYes()
    {
        _current.IsPositiveAnswer = true;
        ButtonHandle();
    }
}