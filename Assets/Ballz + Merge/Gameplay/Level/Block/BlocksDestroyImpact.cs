using BallzMerge.Root.Audio;
using System.Collections;
using UnityEngine;

namespace BallzMerge.Gameplay.BlockSpace
{
    public class BlocksDestroyImpact : MonoBehaviour
    {
        [SerializeField] private AudioSourceHandler _sound;
        [SerializeField] private ParticleSystem _particle;

        public void ShowImpact()
        {
            StartCoroutine(DelaySound());
        }

        private IEnumerator DelaySound()
        {
            yield return new WaitForSeconds(_particle.main.startDelay.constant);
            _sound.Play(AudioEffectsTypes.Hit);
        }
    }
}
