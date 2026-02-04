using TMPro;
using UnityEngine;
using UnityEngine.Localization;

public class BuyFurnitureFrame : MonoBehaviour
{
    public FurnitureController furniture;

    public TMP_Text priceText;

    [Header("Localization")]
    [SerializeField] private LocalizedString priceFormat;

    private void Start()
    {
        Debug.Log("f" + furniture.price);
        priceText.text = priceFormat.GetLocalizedString(
            furniture.price.ToString("F2")
        );
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
