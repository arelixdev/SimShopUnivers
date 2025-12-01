using UnityEngine;
using TMPro;

public class BuyFurnitureObjFrame : MonoBehaviour
{
    public StockObject furnitureObj;

    public TMP_Text priceText;

    private void Start()
    {
        priceText.text = "Price : " + furnitureObj.info.price.ToString("F2") + " €";
    }

    public void BuyFurnitureObj()
    {
        if(StoreController.instance.CheckMoneyAvailable(furnitureObj.info.price))
        {
            StoreController.instance.SpendMoney(furnitureObj.info.price);
            
            Instantiate(furnitureObj.info.stockObject, StoreController.instance.GetStockSpawnPoint().position, Quaternion.identity);
            
        }
    }
}
