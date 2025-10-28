using UnityEngine;

public class SliderBar : MonoBehaviour
{
    [SerializeField] private SliderHandle _sliderHandle;

    private void Awake()
    {
        _sliderHandle.Init();
    }
}