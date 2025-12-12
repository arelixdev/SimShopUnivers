using System.Collections;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MapTooltipsPanel : MonoBehaviour
{
    public static MapTooltipsPanel instance;
    [SerializeField] private Image titleIcn;
    [SerializeField] private TextMeshProUGUI titleTxt;
    [SerializeField] private Image imgElement;
    [SerializeField] private GameObject doorPrivateZone;
    [SerializeField] private Camera renderTextureCamera;
    [SerializeField] private RawImage rawImage;

    [SerializeField] private MapTooltipsSelectElement panelSelectElement;

    [SerializeField] private Vector2 offset = new Vector2(250, 225f);


    private MapTooltipsElement selectedElement;
    private Transform wallElement;

    private bool isFix;

    private void Awake() {
        instance = this;
    }

    private void Start() {
        HideTooltips();
    }

    public void Update()
    {
        if(Mouse.current.rightButton.wasPressedThisFrame && isFix)
        {
            isFix = false;
            HideTooltips();
        }
    }

    public void CustomElementBtn()
    {
        panelSelectElement.ShowMenu(selectedElement.elementType);
    }

    public void ShowTooltips(MapTooltipsElement elem)
    {
        //TODO isFix = false but see if hover an other object != this object
        //isFix = false;
        gameObject.SetActive(true);
        selectedElement = elem;
        SetPosition(elem);
        InitTooltips();
        if(imgElement.sprite == null)
        {
            imgElement.enabled = false;
        }
        UpdateElement();
    }

    public void SetPosition(MapTooltipsElement elem)
    {
        RectTransform tooltipParent = transform.parent as RectTransform;
        RectTransform tooltipRect = GetComponent<RectTransform>();
        RectTransform rawRect = rawImage.rectTransform;
        
        Vector3 viewportPos = renderTextureCamera.WorldToViewportPoint(elem.transform.position);

        Vector2 rawLocalPos = new Vector2(
            (viewportPos.x - 0.5f) * rawRect.rect.width,
            (viewportPos.y - 0.5f) * rawRect.rect.height
        );

        Vector2 screenPos = rawRect.TransformPoint(rawLocalPos);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            tooltipParent,
            screenPos,
            null,
            out Vector2 canvasLocalPos
        );

        tooltipRect.anchoredPosition = canvasLocalPos + offset;
    }

    void InitTooltips()
    {
        titleIcn.sprite = selectedElement.elementSprite;
        titleTxt.text = selectedElement.elementType.ToString();

        doorPrivateZone.SetActive(false);

        switch(selectedElement.elementType)
        {
            case ElementType.Door:
                doorPrivateZone.SetActive(true);
                break;
            default:
                break;
        }

        StartCoroutine(RebuildNextFrame());
    }

    public void HideTooltips()
    {
        if(isFix)
            return;

        gameObject.SetActive(false);
        selectedElement = null;
    }

    public void FixTooltips(Transform wallElem)
    {
        isFix = true;
        wallElement = wallElem;
    }

    public void CreateElement(int val)
    {
        switch(selectedElement.elementType)
        {
            case ElementType.Door:
                imgElement.sprite = StockInfoController.instance.allDoors[val].spriteElement;
                wallElement.GetComponent<BlueprintWallElement>().wallInGame.GetComponent<WallInMallElement>().CreateDoor(val);
                break;
        }

        selectedElement.valueElement = val;
        
        imgElement.enabled = true;
        
        panelSelectElement.HideMenu();
    }

    public void UpdateElement()
    {
        if(selectedElement.valueElement == -1)
            imgElement.sprite = null;
            return;

        switch(selectedElement.elementType)
        {
            case ElementType.Door:
                imgElement.sprite = StockInfoController.instance.allDoors[selectedElement.valueElement].spriteElement;
                break;
        }
    }



    private IEnumerator RebuildNextFrame()
    {
        yield return new WaitForEndOfFrame();

        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }
}
