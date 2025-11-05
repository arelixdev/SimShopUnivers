using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    public GameObject updatePricePanel;

    [SerializeField] private TMP_Text basePriceText, currentPriceText;
    [SerializeField] private TMP_InputField priceInputfield;

    private StockInfo activeStockInfo;

    private void Awake()
    {
        instance = this;
        CloseUpdatePrice();
    }

    public void OpenUpdatePrice(StockInfo stockToUpdate)
    {
        updatePricePanel.SetActive(true);

        basePriceText.text = stockToUpdate.price.ToString("F2") + " €";
        currentPriceText.text = stockToUpdate.currentPrice.ToString("F2") + " €";

        activeStockInfo = stockToUpdate;

        priceInputfield.text = stockToUpdate.currentPrice.ToString("F2");

        Cursor.lockState = CursorLockMode.None;
    }

    public void CloseUpdatePrice()
    {
        updatePricePanel.SetActive(false);

        activeStockInfo = null;

        Cursor.lockState = CursorLockMode.Locked;
    }
    
    public void ApplyPriceUpdate()
    {
        if(priceInputfield.text == "" || priceInputfield.text == string.Empty)
        {
            activeStockInfo.currentPrice = float.Parse(priceInputfield.text);

            currentPriceText.text = activeStockInfo.currentPrice.ToString("F2") + " €";

            StockInfoController.instance.UpdatePrice(activeStockInfo.name, activeStockInfo.currentPrice);

            CloseUpdatePrice();
        }
        
    }
}
