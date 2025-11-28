using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class BuyStockFrame : MonoBehaviour
{
    public StockInfo info;

    [SerializeField] private TMP_Text nameText, priceText, amountInBoxText, boxPriceText, buttonText;

    [SerializeField] private StockBoxController boxToSpawn;

    private float boxCost;

    public void Start()
    {
        UpdateFrameInfo();
    }

    public void UpdateFrameInfo()
    {
        info = StockInfoController.instance.GetInfo(info.name);

        nameText.text = info.name;
        priceText.text = info.price.ToString("F2") + " €";

        int boxAmount = boxToSpawn.GetStockAmount(info.typeOfStock);
        amountInBoxText.text = boxAmount.ToString() + " per box";

        boxCost = (boxAmount * info.price) - 1;
        boxCost = Mathf.Floor(boxCost);
        boxPriceText.text = "Box: " + boxCost.ToString("F2") + " €";

        buttonText.text = "PAY: " + boxCost.ToString("F2") + " €";
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
