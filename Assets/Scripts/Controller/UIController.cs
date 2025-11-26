using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    [SerializeField] private TMP_Text moneyText;

    public GameObject updatePricePanel;
    public GameObject buyMenuScreen;

    public  GameObject wheelTools;

    [SerializeField] private TMP_Text basePriceText, currentPriceText;
    [SerializeField] private TMP_InputField priceInputfield;
    [SerializeField] private GameObject dotPlayer;

    [SerializeField] private TMP_Text lvlGeneralTxt;
    [SerializeField] private Slider lvlGeneralBar;


    [SerializeField] private GameObject shopLvlPanel;
    [SerializeField] private TMP_Text shopNameTxt;
    [SerializeField] private TMP_Text shopLvlTxt;
    [SerializeField] private Slider shopLvlBar;

    public GameObject nameShopScreen;
    [SerializeField] private TMP_InputField nameShopInputfield;

    

    private StockInfo activeStockInfo;

    private void Awake()
    {
        instance = this;
        CloseUpdatePrice();
        buyMenuScreen.SetActive(false);
        shopLvlPanel.SetActive(false);
        nameShopScreen.SetActive(false);
    }

    private void Update() {
        if(Keyboard.current.tabKey.wasPressedThisFrame)
        {
            OpenCloseBuyMenu();
        }
        if(Keyboard.current.tKey.wasPressedThisFrame)
        {
            OpenWheelToolMenu();

        }
        if(Keyboard.current.tKey.wasReleasedThisFrame)
        {
            CloseWheelToolMenu();
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

    public void TooglePlayerDot()
    {
        if(dotPlayer.activeSelf)
        {
            dotPlayer.SetActive(false);
        } else
        {
            dotPlayer.SetActive(true);
        }
    }

    public void UpdateXpGeneralUI(int level, int xpAct)
    {
        lvlGeneralTxt.text = "LVL " + level;
        lvlGeneralBar.maxValue = StoreController.instance.levelXpGeneral[level-1];
        lvlGeneralBar.value = xpAct;
    }

    public void UpdateShopUI(string shopName, int shopLevel, int xpAct)
    {
        if(shopName != "")
        {
            shopLvlPanel.SetActive(true);
            shopNameTxt.text = shopName;
            shopLvlTxt.text = "LVL " + shopLevel;
            shopLvlBar.maxValue = StoreController.instance.GetXpRequiered()[shopLevel-1];
            shopLvlBar.value = xpAct;
        } else
        {
            shopLvlPanel.SetActive(false);
        }
    }

    private ShopZone shopTemp;

    public void OpenCloseNameShop(ShopZone shop)
    {
        shopTemp = shop;

        if(nameShopScreen.activeSelf)
        {
            nameShopScreen.SetActive(false);
            
        } else
        {
            nameShopScreen.SetActive(true);
            nameShopInputfield.text = shop.GetNameShop().ToUpper();
        }
    }

    public void OpenCloseNameShop()
    {
        if(nameShopScreen.activeSelf)
        {
            nameShopScreen.SetActive(false);
            
        }
    }

    public void ValidateShopName()
    {
        shopTemp.SetNameShop(nameShopInputfield.text);
        OpenCloseNameShop();
    }

    public void OpenWheelToolMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        wheelTools.SetActive(true);

        RadialActionContext ctx = new RadialActionContext();
        ctx.player = PlayerController.instance.gameObject;
        ctx.mopHand = PlayerController.instance.mopHand;

        wheelTools.GetComponent<RadialMenuUI>().Open(ctx);
    }

    public void CloseWheelToolMenu()
    {
        Cursor.lockState = CursorLockMode.Locked;
        wheelTools.SetActive(false);
    }
}
