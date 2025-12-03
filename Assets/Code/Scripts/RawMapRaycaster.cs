using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RawMapRaycaster : MonoBehaviour, IPointerClickHandler
{
    public Camera renderTextureCamera;   // La caméra utilisée pour le RenderTexture
    public RawImage rawImage;            // L'image de l'UI

    public LayerMask blueprintLayerMask; 

    public void OnPointerClick(PointerEventData eventData)
    {
        RectTransform rect = rawImage.rectTransform;

        // Position du clic dans le RectTransform
        Vector2 localPoint;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rect,
                eventData.position,
                eventData.pressEventCamera,
                out localPoint))
            return;

        // Convertir en coordonnées normalisées 0-1
        float uvX = (localPoint.x / rect.rect.width) + 0.5f;
        float uvY = (localPoint.y / rect.rect.height) + 0.5f;

        // Convertir les UV en position pixel dans le RenderTexture
        float pixelX = uvX * renderTextureCamera.targetTexture.width;
        float pixelY = uvY * renderTextureCamera.targetTexture.height;

        // Faire le Ray depuis la caméra RT
        Ray ray = renderTextureCamera.ScreenPointToRay(new Vector3(pixelX, pixelY, 0));

        if (Physics.Raycast(ray, out RaycastHit hit, 999f, blueprintLayerMask))
        {
            // ✔ On vérifie aussi le tag  
            if (hit.collider.CompareTag("BlueprintElement"))
            {
                Debug.Log("BlueprintElement cliqué : " + hit.collider.name);
            }
        }
    }
}
