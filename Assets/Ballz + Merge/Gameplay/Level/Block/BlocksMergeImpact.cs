using UnityEngine;

public class BlocksMergeImpact : MonoBehaviour
{
    [SerializeField] private AudioSourceHandler _sound;

    public void ShowImpact()
    {
        _sound.Play();
    }
}