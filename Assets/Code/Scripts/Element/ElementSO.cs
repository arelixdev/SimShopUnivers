using UnityEngine;

[CreateAssetMenu(menuName = "Element/New Element")]
public class ElementSO : ScriptableObject
{
    public ElementType elementType;
    public Sprite spriteElement;
    public GameObject elementPrefab;
}
