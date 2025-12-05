using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RawMapRaycaster : MonoBehaviour, IPointerClickHandler
{
    public Camera renderTextureCamera;
    public RawImage rawImage;
    public LayerMask blueprintLayerMask;

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
            }
        }
    }
}
