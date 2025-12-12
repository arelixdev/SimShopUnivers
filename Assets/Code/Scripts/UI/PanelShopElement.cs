using System.Collections;
using System.Collections.Generic;
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
    List<BlueprintGroundElement> groundElementShop = new List<BlueprintGroundElement>();

    private ShopPlaceableElement element;

    private bool isRetracted;
    private bool isBuy;

    public bool GetIsRetracted()
    {
        return isRetracted;
    }

    public string GetShopName()
    {
        return nameShopInputfield.text;
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
            StartCoroutine(RebuildNextFrame());
            PanelShopMaster.instance.customActivate = true;
        } else
        {
            var tempColor = customBtn.color;
            tempColor.a = 0f;
            customBtn.color = tempColor;

            customLineBuyElement.SetActive(false);
            StartCoroutine(RebuildNextFrame());
            PanelShopMaster.instance.customActivate = false;
        }
        
    }

    private IEnumerator RebuildNextFrame()
    {
        yield return new WaitForEndOfFrame();

        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
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

        PanelShopMaster.instance.RebuildShopMaster();
    }

    public void OnBuyButton()
    {
        if (!PanelShopMaster.instance.CheckSelectionConnectivity())
        {
            Debug.LogError("La sélection n'est pas entièrement connectée !");
            return;
        }

        //TODO faire la partie argent

        groundElementShop = PanelShopMaster.instance.GetCurrentSelection();

        //TODO ajouter tout les mur map et game dans des listes pour pouvoir supprimer
        PanelShopMaster.instance.BuildWallsAroundZone(groundElementShop);

        foreach(var ges in groundElementShop)
        {
            ges.GroundBuy(nameShopInputfield.text);
        }

        PanelShopMaster.instance.ClearSelection();

        ToogleCustom();

        if(groundElementShop.Count > 0)
        {
            addElementPart.SetActive(true);
            PanelShopMaster.instance.lastBoughtZone = new List<BlueprintGroundElement>(groundElementShop);
            isBuy = true;
        }
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
