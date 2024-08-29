using UnityEngine;

public abstract class CyclicBehavior : MonoBehaviour
{
    [Header("---------------------")]
    [SerializeField] private int _initOrder;

    public int Order => _initOrder;
}