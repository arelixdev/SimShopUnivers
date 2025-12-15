using System.ComponentModel;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RawMapRaycaster : MonoBehaviour, IPointerClickHandler
{
    public static RawMapRaycaster instance;

    [SerializeField] private Camera renderTextureCamera;
    [SerializeField] private RawImage rawImage;
    [SerializeField] private LayerMask blueprintLayerMask;

    private BlueprintWallElement lastWallHover = null;
    private MapTooltipsElement lastTooltipHover = null;

    [HideInInspector]
    public ShopPlaceableElement activePlaceable;

    [SerializeField] private MapTooltipsPanel mapTooltipsPanel;

    private bool onClick;
    private bool selected;
    private bool unselected;

    void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        HandleHover();
        if(Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (PanelShopMaster.instance.customActivate)
            {
                onClick = true;
            }
        }

        if(Mouse.current.leftButton.wasReleasedThisFrame)
        {
            selected = false;
            unselected = false;
            onClick = false;
        }
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
            // if(hit.collider != null)
            //     Debug.Log("hit: " + hit.collider.gameObject);
            if (hit.collider.CompareTag("BlueprintElement") && PanelShopMaster.instance.customActivate && onClick)
            {
                var element = hit.collider.GetComponent<BlueprintGroundElement>();
                if (element != null && !element.IsSelected && onClick && !unselected)
                {
                    selected = true;
                    unselected = false;
                    PanelShopMaster.instance.TrySelect(element);
                }
                else if(element != null && element.IsSelected && !selected && onClick)
                {
                    selected = false;
                    unselected = true;
                    PanelShopMaster.instance.RemoveFromSelection(element);
                }
                    
                    
            } 

            var wall = hit.collider.GetComponent<BlueprintWallElement>();

            if (wall != null)
            {
                if (lastWallHover != wall)
                {
                    lastWallHover = wall;

                    if (activePlaceable != null && activePlaceable.GetIsPlacing())
                    {
                        if (!activePlaceable.ignoreWallSnap)
                        {
                            activePlaceable.HideUI();
                            activePlaceable.SnapToWall(wall.transform);
                        }
                    }
                }

                if (lastTooltipHover != null)
                {
                    mapTooltipsPanel.HideTooltips();
                    lastTooltipHover = null;
                }

                return;
            }
            var ground = hit.collider.GetComponent<BlueprintGroundElement>();
            if (ground != null)
            {
                // Si l’élément actif existe et ignore le snap mur (donc PlacementWallElement)
                if (activePlaceable != null && activePlaceable.GetIsPlacing() && activePlaceable.ignoreWallSnap)
                {
                    activePlaceable.OnGroundHover(ground);
                    return;
                }
            }

            var pivot = hit.collider.GetComponent<WallConstructorPivot>();
            if (pivot != null)
            {
                if (activePlaceable != null && activePlaceable.GetIsPlacing() && activePlaceable.ignoreWallSnap)
                {
                    (activePlaceable as PlacementWallElement)?.OnPivotHover(pivot);
                    return;
                }
            }
            var tooltipElement = hit.collider.GetComponent<MapTooltipsElement>();

            if (tooltipElement != null && activePlaceable == null)
            {
                // Si on change d'élément tooltip
                if (lastTooltipHover != tooltipElement)
                {
                    lastTooltipHover = tooltipElement;
                    mapTooltipsPanel.ShowTooltips(tooltipElement);
                }
                return;
            }
        }

        if (lastTooltipHover != null)
        {
            mapTooltipsPanel.HideTooltips();
            lastTooltipHover = null;
        }

        if (lastWallHover != null)
        {
            lastWallHover = null;

            if (activePlaceable != null && activePlaceable.GetIsPlacing())
            {
                if (!activePlaceable.ignoreWallSnap)
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

             var pivot = hit.collider.GetComponent<WallConstructorPivot>();
            if (pivot != null && activePlaceable is PlacementWallElement lwe)
            {
                lwe.SelectPivot(pivot);
                return;
            }

            //Select element (door / window / wall)
            if(hit.collider.CompareTag("BlueprintElement") && PanelShopMaster.instance.deleteToolActive)
            {
                
                var element = hit.collider.GetComponent<MapTooltipsElement>();

                if(element != null)
                    element.ClearElement();

                return;
            }
            
             else if(hit.collider.CompareTag("BlueprintElement") && hit.collider.GetComponent<BlueprintWallElement>() != null)
            {
                if (activePlaceable != null)
                {
                    activePlaceable.TryPlace();
                    return;
                }
            } else if (hit.collider.CompareTag("BlueprintElement") && hit.collider.GetComponent<MapTooltipsElement>() != null)
            {
                mapTooltipsPanel.FixTooltips(hit.collider.GetComponent<MapTooltipsElement>().wallElement);
            }
        }

        
    }

    public void CleanActivePlaceable()
    {
        Destroy(activePlaceable.gameObject);
        activePlaceable = null;
    }
}
