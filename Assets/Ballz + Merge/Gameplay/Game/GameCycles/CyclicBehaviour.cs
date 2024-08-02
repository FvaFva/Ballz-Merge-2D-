using UnityEngine;

public abstract class CyclicBehaviour : MonoBehaviour
{
    [SerializeField] private int _initOrder;
    [Header("---------------------")]

    public int Order => _initOrder;
}