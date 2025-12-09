using System.Collections;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MapTooltipsPanel : MonoBehaviour
{
    [SerializeField] private Image titleIcn;
    [SerializeField] private TextMeshProUGUI titleTxt;
    [SerializeField] private GameObject doorPrivateZone;
    [SerializeField] private Camera renderTextureCamera;
    [SerializeField] private RawImage rawImage;

    [SerializeField] private Vector2 offset = new Vector2(250, 225f);


    private MapTooltipsElement selectedElement;
    private bool isFix;

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
        Debug.Log("Custom panel open");
    }

    public void ShowTooltips(MapTooltipsElement elem)
    {
        //TODO isFix = false but see if hover an other object != this object
        //isFix = false;
        gameObject.SetActive(true);
        selectedElement = elem;
        SetPosition(elem);
        InitTooltips();

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

    public void FixTooltips()
    {
        isFix = true;
    }

    private IEnumerator RebuildNextFrame()
    {
        yield return new WaitForEndOfFrame();

        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }
}
