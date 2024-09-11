using UnityEngine.Events;
using UnityEngine.UI;

public static class ButtonExtension
{
    public static void AddListener(this Button button, UnityAction coll) => button.onClick.AddListener(coll);
    public static void RemoveListener(this Button button, UnityAction coll) => button.onClick.RemoveListener(coll);
    public static void ChangeListeningState(this Button button, UnityAction coll, bool isActive)
    {
        if(isActive)
            AddListener(button, coll);
        else 
            RemoveListener(button, coll);
    }
}