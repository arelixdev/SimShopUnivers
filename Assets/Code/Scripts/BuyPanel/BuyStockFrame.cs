using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UIElements;

public class BuyStockFrame : MonoBehaviour
{
    public StockInfoSO info;

    [SerializeField] private TMP_Text nameText, priceText, amountInBoxText, boxPriceText, buttonText;

    [SerializeField] private StockBoxController boxToSpawn;

    [Header("Localization")]
    [SerializeField] private LocalizedString perBoxFormat;
    [SerializeField] private LocalizedString boxPriceFormat;
    [SerializeField] private LocalizedString payPriceFormat;

    private float boxCost;

    public void AddStockFrame(StockInfoSO obj)
    {
        info = obj;
        UpdateFrameInfo();
    }

    public void UpdateFrameInfo()
    {
        info = StockInfoController.instance.GetInfo(info.name);

        nameText.text = info.name;
        priceText.text = info.price.ToString("F2") + " €";

        int boxAmount = boxToSpawn.GetStockAmount(info.typeOfStock);
        amountInBoxText.text = perBoxFormat.GetLocalizedString(boxAmount);

        boxCost = (boxAmount * info.price) - 1;

        boxCost = Mathf.Floor(boxCost);
        boxPriceText.text = boxPriceFormat.GetLocalizedString(boxCost.ToString("F2"));
        buttonText.text = payPriceFormat.GetLocalizedString(boxCost.ToString("F2"));
    }

    public void BuyBox()
    {
        if(StoreController.instance.CheckMoneyAvailable(boxCost))
        {
            StoreController.instance.SpendMoney(boxCost);

            Instantiate(boxToSpawn, StoreController.instance.GetStockSpawnPoint().position, Quaternion.identity).SetupBox(info);
        }
    }
}
