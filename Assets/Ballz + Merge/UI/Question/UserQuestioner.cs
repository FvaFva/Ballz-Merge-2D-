using BallzMerge.Root;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class UserQuestioner : MonoBehaviour
{
    [SerializeField] private TMP_Text _label;
    [SerializeField] private Button _yes;
    [SerializeField] private Button _no;

    [Inject] IGameTimeOwner _pauseController;

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

    public void Show(UserQuestion question)
    {
        if (_current.IsEmpty())
        {
            _pauseController.Stop();
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
        if (_questions.TryDequeue(out _current))
            Display(_current);
        else
            Hide();
    }

    private void Hide()
    {
        _current = default;
        _pauseController.SetRegular();
        _label.text = string.Empty;
        gameObject.SetActive(false);
    }

    private void OnAnswerNo()
    {
        _current.CallBack(false);
        ButtonHandle();
    }

    private void OnAnswerYes()
    {
        _current.CallBack(true);
        ButtonHandle();
    }
}