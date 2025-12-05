using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelShopElement : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameShopInputfield;
    [SerializeField] private Image customBtn;
    [SerializeField] private GameObject customLineBuyElement;
    [SerializeField] private GameObject addElementPart;


    List<BlueprintGroundElement> groundElementShop = new List<BlueprintGroundElement>();
    

    void Start()
    {
        InitializePanelShop();
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

    public void OnBuyButton()
    {
        if (!PanelShopMaster.instance.CheckSelectionConnectivity())
        {
            Debug.LogError("La sélection n'est pas entièrement connectée !");
            return;
        }

        //TODO faire la partie argent
        //TODO construire les murs ! 

        groundElementShop = PanelShopMaster.instance.GetCurrentSelection();

        PanelShopMaster.instance.BuildWallsAroundZone(groundElementShop);

        foreach(var ges in groundElementShop)
        {
            ges.GroundBuy();
        }

        PanelShopMaster.instance.ClearSelection();

        ToogleCustom();

        if(groundElementShop.Count > 0)
        {
            addElementPart.SetActive(true);
        }
    }
}
