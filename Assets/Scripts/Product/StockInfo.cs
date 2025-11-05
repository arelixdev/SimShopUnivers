using System;

[Serializable]
public class StockInfo
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
