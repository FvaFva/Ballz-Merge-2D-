using BallzMerge.Root;
using System;
using System.Collections;
using UnityEngine;

public class ValueChanger
{
    private const float DefaultDuration = 0.1f;

    private Coroutine _coroutine;

    public void Stop()
    {
        if (_coroutine != null)
        {
            CoroutineRunner.Instance.StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }

    public void ChangeValueOverTime(float from, float to, Action<float> onValueChanged, Action OnEndChanging = null, float duration = DefaultDuration)
    {
        Stop();
        _coroutine = CoroutineRunner.Instance.StartCoroutine(ChangingValue(from, to, onValueChanged, OnEndChanging, duration));
    }

    private IEnumerator ChangingValue(float from, float to, Action<float> onValueChanged, Action OnEndChanging, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            float newValue = Mathf.Lerp(from, to, Mathf.SmoothStep(0f, 1f, t));
            onValueChanged(newValue);
            yield return null;
        }

        onValueChanged(to);
        OnEndChanging?.Invoke();
        _coroutine = null;
    }
}