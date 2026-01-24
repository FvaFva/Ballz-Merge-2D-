using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Zenject;

public class ScrollViewMouseScroll : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private float _scrollStep = 0.1f;

    [Inject] private MainInputMap _userInput;

    public void OnPointerEnter(PointerEventData eventData)
    {
        _userInput.MainInput.Scroll.performed += ChangePosition;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _userInput.MainInput.Scroll.performed -= ChangePosition;
    }

    private void ChangePosition(InputAction.CallbackContext context)
    {
        Vector2 scroll = context.ReadValue<Vector2>();

        if (scroll.y != 0)
        {
            _scrollRect.verticalNormalizedPosition += Mathf.Sign(scroll.y) * _scrollStep;
            _scrollRect.verticalNormalizedPosition = Mathf.Clamp01(_scrollRect.verticalNormalizedPosition);
        }
    }
}
