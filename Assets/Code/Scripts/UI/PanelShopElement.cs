using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PanelShopElement : MonoBehaviour
{
    [SerializeField] private Image retractBtn;
    [SerializeField] private TMP_Text retractTxt;
    [SerializeField] private TMP_InputField nameShopInputfield;

    [SerializeField] private GameObject customElement;
    [SerializeField] private Image customBtn;
    [SerializeField] private GameObject customLineBuyElement;
    [SerializeField] private GameObject shopTypeElement;
    [SerializeField] private GameObject addElementPart;
    [SerializeField] private GameObject sellBtn;
    List<BlueprintGroundElement> groundElementShop = new List<BlueprintGroundElement>();
    public List<GameObject> allWallShop = new List<GameObject>();
    public List<GameObject> allWallGameShop = new List<GameObject>();

    public List<MapTooltipsElement> allShopElement = new List<MapTooltipsElement>();

    public List<GameObject> allElement = new List<GameObject>();

    public List<WallKey> allWallKeys = new List<WallKey>();

    private ShopPlaceableElement element;

    private bool selectInputfield;

    private bool isRetracted;
    private bool isBuy;

    public bool HasBoughtZone()
    {
        return groundElementShop != null && groundElementShop.Count > 0;
    }

    public List<BlueprintGroundElement> GetGroundElements()
    {
        return groundElementShop;
    }

    public void RemoveGround(BlueprintGroundElement ground)
    {
        if (groundElementShop.Contains(ground))
        {
            groundElementShop.Remove(ground);
        }
    }

    public bool GetIsRetracted()
    {
        return isRetracted;
    }

    public string GetShopName()
    {
        return nameShopInputfield.text;
    }

    public void ToogleSelectInputfield()
    {
        selectInputfield = !selectInputfield;

        UIController.instance.InputfieldSelected(selectInputfield);
    }

    public void ClearShop()
    {
        for (int i = groundElementShop.Count-1; i >= 0 ; i--)
        {
            groundElementShop[i].CleanGround();
        }

        for(int i = allWallShop.Count-1; i >= 0; i--)
        {
            Destroy(allWallShop[i]);
        }

        for(int i = allWallGameShop.Count-1; i >= 0; i--)
        {
            Destroy(allWallGameShop[i]);
        }

        for(int i = allShopElement.Count -1; i >= 0; i--)
        {
            allShopElement[i].ClearElement();
        }

        for(int i = allElement.Count-1; i >= 0; i--)
        {
            Destroy(allElement[i]);
        }

        foreach (var key in allWallKeys)
        {
            PanelShopMaster.instance.createdWalls.Remove(key);
        }



        groundElementShop.Clear();
        allWallShop.Clear();
        allWallGameShop.Clear();
        allElement.Clear();

        PanelShopMaster.instance.RebuildAllShopWalls();
    }
    

    void Start()
    {
        InitializePanelShop();
    }

    void Update()
    {
        if(Mouse.current.rightButton.wasPressedThisFrame && element != null)
        {
            element.ClearElement();
            PanelShopMaster.instance.deleteToolActive = false;
        }
    }

    void InitializePanelShop()
    {
        customLineBuyElement.SetActive(false);
        addElementPart.SetActive(false);
        sellBtn.SetActive(false);

        var tempColor = customBtn.color;
        tempColor.a = 0f;
        customBtn.color = tempColor;
    }

    public void ToogleCustom()
    {
        if(customBtn.color.a == 0)
        {
            var tempColor = customBtn.color;
            tempColor.a = 1f;
            customBtn.color = tempColor;

            customLineBuyElement.SetActive(true);
            UILayoutRebuildManager.instance.RequestRebuild(
                GetComponent<RectTransform>()
            );
            PanelShopMaster.instance.customActivate = true;
        } else
        {
            var tempColor = customBtn.color;
            tempColor.a = 0f;
            customBtn.color = tempColor;

            customLineBuyElement.SetActive(false);
            UILayoutRebuildManager.instance.RequestRebuild(
                GetComponent<RectTransform>()
            );
            PanelShopMaster.instance.customActivate = false;
        }
        
    }

    private IEnumerator RebuildNextFrame()
    {
        var canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;

        yield return new WaitForEndOfFrame();
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());

        yield return null;
        canvasGroup.alpha = 1f;
    }

    public void TooglePanelShop()
    {
        //If retract
        isRetracted = !isRetracted;

        if(isRetracted)
        {
            customElement.SetActive(false);
            shopTypeElement.SetActive(false);
            addElementPart.SetActive(false);

            var tempColor = GetComponent<Image>().color;
            tempColor.a = 0f;
            GetComponent<Image>().color = tempColor;

            tempColor = retractBtn.color;
            tempColor.a = 0f;
            retractBtn.color = tempColor;

            retractTxt.text = ">";

            nameShopInputfield.interactable = false;
        } else
        {
            customElement.SetActive(true);
            shopTypeElement.SetActive(true);
            if(!isBuy)
                addElementPart.SetActive(false);
            else
                addElementPart.SetActive(true);

            var tempColor = GetComponent<Image>().color;
            tempColor.a = 1f;
            GetComponent<Image>().color = tempColor;

            PanelShopMaster.instance.ChangePanelSelected(gameObject);

            tempColor = retractBtn.color;
            tempColor.a = 1f;
            retractBtn.color = tempColor;

            retractTxt.text = "<";

            nameShopInputfield.interactable = true;
        }
        
        //if deploy

        UILayoutRebuildManager.instance.RequestRebuild(
            GetComponent<RectTransform>()
        );
    }

    public void SellBtn()
    {
        PanelShopMaster.instance.ShowSellPanel();
    }

    public void OnBuyButton()
    {
        if (!PanelShopMaster.instance.CheckSelectionConnectivity())
        {
            Debug.LogError("La sélection n'est pas entièrement connectée !");
            return;
        }

        var selection = PanelShopMaster.instance.GetCurrentSelection();

        if (selection.Count == 0)
            return;

        if (!PanelShopMaster.instance.SelectionContainsMall(selection))
        {
            Debug.LogError("La sélection doit contenir au moins une case connectée au mall !");
            return;
        }

        // FIRST BUY
        if (!isBuy)
        {
            groundElementShop = new List<BlueprintGroundElement>(selection);

            PanelShopMaster.instance.BuildWallsAroundZone(groundElementShop, this);

            foreach (var tile in groundElementShop)
                tile.GroundBuy(nameShopInputfield.text);

            isBuy = true;
            sellBtn.SetActive(true);
            addElementPart.SetActive(true);
        }
        // EXTENSION
        else
        {
            
            foreach (var tile in selection)
            {
                if (!groundElementShop.Contains(tile))
                {
                    groundElementShop.Add(tile);
                    tile.GroundBuy(nameShopInputfield.text);
                }
            }

        
            RebuildShopWalls();
        }

        PanelShopMaster.instance.ClearSelection();
        ToogleCustom();
    }

    public void RebuildShopWalls()
    {
        foreach (var key in allWallKeys)
        {
            PanelShopMaster.instance.createdWalls.Remove(key);
        }

        allWallKeys.Clear();

        foreach (var wall in allWallShop)
            Destroy(wall);

        foreach (var wall in allWallGameShop)
            Destroy(wall);

        allWallShop.Clear();
        allWallGameShop.Clear();

        PanelShopMaster.instance.BuildWallsAroundZone(groundElementShop, this);
    }

    public void AddElementBtn(string typeButton)
    {
        var prefab = PanelShopMaster.instance.GetElementById(typeButton);

        if (prefab == null)
            return;

        element = Instantiate(prefab);

        if(typeButton == "Delete")
            PanelShopMaster.instance.deleteToolActive = true;


        element.Init(
            uiParent: PanelShopMaster.instance.mapMenuPanel,
            planParent: PanelShopMaster.instance.planParent
        );

        if(typeButton != "Delete")
            PanelShopMaster.instance.SetTooltipWallsColliders(true);

        element.StartPlacing();

        element.onPlaced += () =>
        {
            PanelShopMaster.instance.SetTooltipWallsColliders(false);
        };

    }
}
