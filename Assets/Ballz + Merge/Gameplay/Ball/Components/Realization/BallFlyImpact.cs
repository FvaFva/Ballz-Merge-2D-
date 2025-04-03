using BallzMerge.Gameplay.BallSpace;
using BallzMerge.Root.Audio;
using UnityEngine;

public class BallFlyImpact : BallComponent
{
    [SerializeField] private Ball _ball;
    [SerializeField] private AudioSourceHandler _audio;

    public void OnEnable()
    {
        _ball.EnterGame += ShowImpact;
    }

    public void OnDisable()
    {
        _ball.EnterGame -= ShowImpact;
    }

    private void ShowImpact()
    {
        _audio.Play(AudioEffectsTypes.Move);
    }
}
