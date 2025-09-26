using BallzMerge.Gameplay.Level;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GlobalEffects : MonoBehaviour
{
    [SerializeField] private List<ParticleSystem> _particles;
    [SerializeField] private List<ParticleSystem> _boardsEffects;

    public IReadOnlyList<string> AllEffects => _particles.Select(p => p.name).ToList();

    public void ChangeState(bool state, string name)
    {
        foreach (var particle in _particles)
        {
            if (particle.name == name)
            {
                particle.gameObject.SetActive(state);
                return;
            }
        }
    }
}
