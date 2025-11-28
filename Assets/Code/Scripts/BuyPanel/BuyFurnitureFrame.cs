using TMPro;
using UnityEngine;

public class BuyFurnitureFrame : MonoBehaviour
{
    public FurnitureController furniture;

    public TMP_Text priceText;

    private void Start()
    {
        priceText.text = "Price : " + furniture.price.ToString("F2") + " €";
    }
    
    public void BuyFurniture()
    {
        if(StoreController.instance.CheckMoneyAvailable(furniture.price))
        {
            StoreController.instance.SpendMoney(furniture.price);

            Instantiate(furniture, StoreController.instance.GetFurnitureSpawnPoint().position, Quaternion.identity);
        }
    }
}
