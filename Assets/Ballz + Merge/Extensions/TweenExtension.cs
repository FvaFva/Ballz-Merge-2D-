using DG.Tweening;

public static class TweenExtension
{
    public static void Delete(this Tween target)
    {
        if (target != null && target.IsActive())
            target.Kill();
    }
}
