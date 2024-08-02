using UnityEngine;

public abstract class CyclicBehaviour : MonoBehaviour
{
    [SerializeField] private int _initOrder;

    public int Order => _initOrder;

    public abstract void Init();
}