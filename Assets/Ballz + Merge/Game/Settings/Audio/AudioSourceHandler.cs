using System.Collections.Generic;
using UnityEngine;

namespace BallzMerge.Root.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioSourceHandler : MonoBehaviour
    {
        private const float PitchDice = 0.2f;

        [SerializeField] private List<AudioEffectBind> _effects;

        private AudioSource _audio;
        private Dictionary<AudioEffectsTypes, AudioClip> _effectsDictionary;

        private void Awake()
        {
            _audio = GetComponent<AudioSource>();
            _effectsDictionary = new Dictionary<AudioEffectsTypes, AudioClip>();

            foreach (var effect in _effects)
                _effectsDictionary[effect.Type] = effect.Effect;
        }

        public void Play(AudioEffectsTypes type) => Play(_effectsDictionary[type]);

        public void Play(int soundID = 0) => Play(_effects[soundID].Effect);

        private void Play(AudioClip clip)
        {
            _audio.pitch = Random.Range(1 - PitchDice, 1 + PitchDice);
            _audio.PlayOneShot(clip);
        }
    }
}