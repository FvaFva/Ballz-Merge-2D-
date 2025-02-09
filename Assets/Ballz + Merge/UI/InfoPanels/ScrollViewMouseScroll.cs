using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Zenject;

public class ScrollViewMouseScroll : MonoBehaviour
{
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private float scrollSpeed = 5f;

    [Inject] private MainInputMap _userInput;

    private void OnEnable()
    {
        _userInput.MainInput.Scroll.performed += ChangePosition;
    }

    private void OnDisable()
    {
        _userInput.MainInput.Scroll.performed -= ChangePosition;
    }

    private void ChangePosition(InputAction.CallbackContext context)
    {
        Vector2 scroll = context.ReadValue<Vector2>();

        if (scroll.y != 0)
        {
            _scrollRect.verticalNormalizedPosition += scroll.y * scrollSpeed * Time.deltaTime;
            _scrollRect.verticalNormalizedPosition = Mathf.Clamp01(_scrollRect.verticalNormalizedPosition);
        }
    }
}
