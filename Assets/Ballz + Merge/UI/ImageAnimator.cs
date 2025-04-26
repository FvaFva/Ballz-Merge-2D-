using UnityEngine;

public class ImageAnimator : MonoBehaviour
{
    private const float Duration = 0.125f;
    private const float HighlightValue = 1.05f;
    private const float PressValue = 0.9f;
    private const float StartScale = 1f;
    private const float StartShadow = 0f;
    private const float EndShadow = 1f;

    [SerializeField] private ImageView _imageView;
    [SerializeField] private ImageView _imageScale;

    public void DoDefault()
    {
        _imageView.SetSize(StartScale, Duration);
        _imageView.SetShadow(StartShadow, Duration);
        _imageScale.SetSize(StartScale, Duration);
    }

    public void SetDefault()
    {
        _imageView.SetSize(new Vector2(StartScale, StartScale));
        _imageView.SetShadow(StartShadow);
        _imageScale.SetSize(new Vector2(StartScale, StartScale));
    }

    public void Highlight()
    {
        _imageScale.SetSize(HighlightValue, Duration);
        _imageView.SetShadow(EndShadow, Duration);
    }

    public void Press()
    {
        _imageView.SetSize(PressValue, Duration);
        _imageView.SetShadow(StartShadow, Duration);
    }
}