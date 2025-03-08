using BallzMerge.Achievement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementDisplayer : MonoBehaviour
{
    private RectTransform _container;
    private AchievementView _achievementView;
    private List<RectTransform> _activePopups = new List<RectTransform>();
    private string _currentLabel;
    private string _currentDescription;
    private Sprite _currentImage;

    public float popupDuration = 2f; // Длительность показа окна
    public float moveSpeed = 5f;    // Скорость движения окна
    public float verticalSpacing = 10f; // Расстояние между окнами

    public AchievementDisplayer Init(AchievementView achievementView, RectTransform container)
    {
        _achievementView = achievementView;
        _container = container;
        return this;
    }

    public void SpawnView(string label, string description, Sprite image)
    {
        _currentLabel = label;
        _currentDescription = description;
        _currentImage = image;
        StartCoroutine(ShowAchievementCoroutine());
    }

    private IEnumerator ShowAchievementCoroutine()
    {
        // Создаем новое всплывающее окно
        AchievementView achievementView = Instantiate(_achievementView, _container);
        achievementView.SetData(_currentLabel, _currentDescription, _currentImage);
        RectTransform popupRect = achievementView.RectTransform;

        // Вычисляем начальную позицию для нового окна (начинаем снизу за экраном)
        float startPos = -popupRect.rect.height; // Начальная позиция — за экраном снизу
        popupRect.anchoredPosition = new Vector2(popupRect.anchoredPosition.x, startPos);

        // Перемещаем все текущие окна вниз, чтобы освободить место
        ShiftPopupsDown();

        // Анимация появления нового окна
        float elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            popupRect.anchoredPosition = new Vector2(popupRect.anchoredPosition.x,
                Mathf.Lerp(startPos, Screen.height - popupRect.rect.height / 2, elapsedTime));
            elapsedTime += Time.deltaTime * moveSpeed;
            yield return null;
        }

        // Устанавливаем конечную позицию окна
        popupRect.anchoredPosition = new Vector2(popupRect.anchoredPosition.x, Screen.height - popupRect.rect.height / 2);

        // Добавляем окно в список активных
        _activePopups.Add(popupRect);

        // Ждем заданное время
        yield return new WaitForSeconds(popupDuration);

        // Анимация исчезновения окна
        elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            popupRect.anchoredPosition = new Vector2(popupRect.anchoredPosition.x,
                Mathf.Lerp(Screen.height - popupRect.rect.height / 2, -popupRect.rect.height, elapsedTime));
            elapsedTime += Time.deltaTime * moveSpeed;
            yield return null;
        }

        // Убираем окно из списка и уничтожаем его
        _activePopups.Remove(popupRect);
        Destroy(achievementView);

        // Перемещаем оставшиеся окна обратно, чтобы вернуть их в исходное положение
        ShiftPopupsUp();
    }

    // Функция сдвигает все окна вниз
    private void ShiftPopupsDown()
    {
        foreach (var popup in _activePopups)
        {
            popup.anchoredPosition = new Vector2(popup.anchoredPosition.x,
                popup.anchoredPosition.y - (popup.rect.height + verticalSpacing));
        }
    }

    // Функция сдвигает все окна вверх после исчезновения
    private void ShiftPopupsUp()
    {
        foreach (var popup in _activePopups)
        {
            popup.anchoredPosition = new Vector2(popup.anchoredPosition.x,
                popup.anchoredPosition.y + (popup.rect.height + verticalSpacing));
        }
    }
}