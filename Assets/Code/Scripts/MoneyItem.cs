using UnityEngine;

public class MoneyItem : MonoBehaviour
{
    [SerializeField] private float value; // ex : 0.50f, 1f, 2f, 5f etc.

    public float GetValue()
    {
        return value;
    }
}
