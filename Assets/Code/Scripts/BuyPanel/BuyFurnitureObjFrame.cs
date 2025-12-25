using UnityEngine;
using TMPro;
using UnityEngine.Localization;

public class BuyFurnitureObjFrame : MonoBehaviour
{
    public StockObject furnitureObj;

    public TMP_Text priceText;

    [Header("Localization")]
    [SerializeField] private LocalizedString priceFormat;

    private void Start()
    {
        priceText.text = priceFormat.GetLocalizedString(
            furnitureObj.info.price.ToString("F2")
        );
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
