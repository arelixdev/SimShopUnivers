using UnityEngine;

[CreateAssetMenu(menuName = "Object/New Object")]
public class StockInfoSO : ScriptableObject
{
    public string name;

    public enum StockType
    {
        cereal,
        drink,
        fruit,
    }

    public StockType typeOfStock;
    public float price;
    public float currentPrice;

    public StockObject stockObject;
}
