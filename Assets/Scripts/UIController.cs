using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    [SerializeField] private TMP_Text moneyText;

    public GameObject updatePricePanel;
    public GameObject buyMenuScreen;

    [SerializeField] private TMP_Text basePriceText, currentPriceText;
    [SerializeField] private TMP_InputField priceInputfield;

    

    private StockInfo activeStockInfo;

    private void Awake()
    {
        instance = this;
        CloseUpdatePrice();
        buyMenuScreen.SetActive(false);
    }

    private void Update() {
        if(Keyboard.current.tabKey.wasPressedThisFrame)
        {
            OpenCloseBuyMenu();
        }
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
        activeStockInfo.currentPrice = float.Parse(priceInputfield.text);

        currentPriceText.text = activeStockInfo.currentPrice.ToString("F2") + " €";

        StockInfoController.instance.UpdatePrice(activeStockInfo.name, activeStockInfo.currentPrice);

        CloseUpdatePrice();

    }

    public void UpdateMoney(float currentMoney)
    {
        moneyText.text = currentMoney.ToString("F2") + " €";
    }
    
    public void OpenCloseBuyMenu()
    {
        if(buyMenuScreen.activeSelf)
        {
            buyMenuScreen.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
        } else
        {
            buyMenuScreen.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
