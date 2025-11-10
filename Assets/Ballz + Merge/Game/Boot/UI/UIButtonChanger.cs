using UnityEngine;
using System.Collections.Generic;

public class UIButtonChanger : CyclicBehavior, IDependentScreenOrientation
{
    [SerializeField] private Sprite _horizontalSprite;
    [SerializeField] private Sprite _verticalSprite;
    [SerializeField] private List<AnimatedButton> _buttons;

    public void UpdateScreenOrientation(bool isVertical)
    {
        if (isVertical)
            ChangeSprite(_verticalSprite);
        else
            ChangeSprite(_horizontalSprite);
    }

    private void ChangeSprite(Sprite sprite)
    {
        foreach (var button in _buttons)
            button.ChangeSprite(sprite);
    }
}
