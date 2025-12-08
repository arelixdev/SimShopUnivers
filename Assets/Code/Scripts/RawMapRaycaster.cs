using System.ComponentModel;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RawMapRaycaster : MonoBehaviour, IPointerClickHandler
{
    public static RawMapRaycaster instance;

    public Camera renderTextureCamera;
    public RawImage rawImage;
    public LayerMask blueprintLayerMask;

    private BlueprintWallElement lastWallHover = null;

    public ShopPlaceableElement activePlaceable;

    void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        HandleHover();
    }

    private void HandleHover()
    {
        if (renderTextureCamera == null || rawImage == null)
        return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        RectTransform rect = rawImage.rectTransform;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, mousePos, null, out Vector2 localPoint))
            return;

        if (!RectTransformUtility.RectangleContainsScreenPoint(rect, mousePos))
            return;

        float uvX = (localPoint.x / rect.rect.width) + 0.5f;
        float uvY = (localPoint.y / rect.rect.height) + 0.5f;

        float pixelX = uvX * renderTextureCamera.targetTexture.width;
        float pixelY = uvY * renderTextureCamera.targetTexture.height;


        Ray ray = renderTextureCamera.ScreenPointToRay(new Vector3(pixelX, pixelY, 0));

        if (Physics.Raycast(ray, out RaycastHit hit, 999f, blueprintLayerMask))
        {
            var wall = hit.collider.GetComponent<BlueprintWallElement>();

            if (wall != null)
            {

                if (lastWallHover != wall)
                {
                    lastWallHover = wall;

                    if (activePlaceable != null && activePlaceable.GetIsPlacing())
                    {
                        activePlaceable.HideUI(); 
                        activePlaceable.SnapToWall(wall.transform); 
                    }
                }

                return;
            }
        }

        if (lastWallHover != null)
        {
            lastWallHover = null;

            if (activePlaceable != null && activePlaceable.GetIsPlacing()  )
            {
                activePlaceable.ShowUI(); 
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        RectTransform rect = rawImage.rectTransform;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rect,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 localPoint))
            return;

        float uvX = (localPoint.x / rect.rect.width) + 0.5f;
        float uvY = (localPoint.y / rect.rect.height) + 0.5f;

        float pixelX = uvX * renderTextureCamera.targetTexture.width;
        float pixelY = uvY * renderTextureCamera.targetTexture.height;

        Ray ray = renderTextureCamera.ScreenPointToRay(new Vector3(pixelX, pixelY, 0));

        if (Physics.Raycast(ray, out RaycastHit hit, 999f, blueprintLayerMask))
        {
            if (hit.collider.CompareTag("BlueprintElement") && PanelShopMaster.instance.customActivate)
            {
                var element = hit.collider.GetComponent<BlueprintGroundElement>();
                if (element != null)
                    PanelShopMaster.instance.TrySelect(element);
            } else if(hit.collider.CompareTag("BlueprintElement") && hit.collider.GetComponent<BlueprintWallElement>() != null)
            {
                if (activePlaceable != null)
                {
                    activePlaceable.TryPlace();
                    return;
                }
            }
        }
    }
}
