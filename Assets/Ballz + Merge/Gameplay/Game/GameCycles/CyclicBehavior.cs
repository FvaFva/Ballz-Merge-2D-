using UnityEngine;

public abstract class CyclicBehavior : MonoBehaviour
{
    [Header("---------------------")]
    [SerializeField] private int _initOrder;
    [SerializeField] private int _initWeight = 1;

    public int Order => _initOrder;
}