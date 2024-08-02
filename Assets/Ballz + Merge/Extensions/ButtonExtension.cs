using UnityEngine.Events;
using UnityEngine.UI;

public static class ButtonExtension
{
    public static void AddListener(this Button button, UnityAction coll) => button.onClick.AddListener(coll);
    public static void RemoveListener(this Button button, UnityAction coll) => button.onClick.RemoveListener(coll);
}