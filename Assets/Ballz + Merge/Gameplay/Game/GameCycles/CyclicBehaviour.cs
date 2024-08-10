using UnityEngine;

public abstract class CyclicBehaviour : MonoBehaviour
{
    [Header("---------------------")]
    [SerializeField] private int _initOrder;

    public int Order => _initOrder;
}