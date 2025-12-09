using UnityEngine;
using UnityEngine.InputSystem;

public abstract class ShopPlaceableElement : MonoBehaviour
{
    public GameObject uiPrefab;
    public GameObject planPrefab;

    [SerializeField] private ElementType typeOfElement;

    protected GameObject uiInstance;
    protected GameObject planInstance;

    protected bool isPlacing;

    protected Transform uiParent;
    protected Transform planParent;

    protected bool isSnappingToWall = false;
    protected Transform currentWallSnap = null;

    public bool GetIsPlacing()
    {
        return isPlacing;
    }

    public virtual void Init(Transform uiParent, Transform planParent)
    {
        this.uiParent = uiParent;
        this.planParent = planParent;
    }

    public virtual void StartPlacing()
    {
        if (uiPrefab != null)
            uiInstance = Instantiate(uiPrefab, uiParent);

        if (planPrefab != null)
        {
            planInstance = Instantiate(planPrefab, planParent);
            planInstance.SetActive(false); 
        }

        RawMapRaycaster.instance.activePlaceable = this;
            

        isPlacing = true;
    }

    protected virtual void Update()
    {
        if (isPlacing)
            FollowMouse();
    }

    protected virtual void FollowMouse()
{
    if (Mouse.current == null)
        return;

    Vector2 mousePos = Mouse.current.position.ReadValue();

    // UI
    if (uiInstance != null)
        uiInstance.transform.position = mousePos;

    // 3D PLAN
    if (isSnappingToWall && currentWallSnap != null)
    {
        // Si on est snapé, on ne suit pas la souris
        SnapToWall(currentWallSnap);
        return;
    }

    Ray ray = Camera.main.ScreenPointToRay(mousePos);

    if (Physics.Raycast(ray, out RaycastHit hit, 1000f, LayerMask.GetMask("BluePrint")))
    {
        if (planInstance != null)
            planInstance.transform.position = hit.point;
    }
}

    public virtual void ShowUI()
    {
        isSnappingToWall = false;
        currentWallSnap = null;

        if (uiInstance != null)
            uiInstance.SetActive(true);

        if (planInstance != null)
            planInstance.SetActive(false);
    }

    public virtual void HideUI()
    {
        if (uiInstance != null)
            uiInstance.SetActive(false);

        if (planInstance != null)
            planInstance.SetActive(true);
    }

    public virtual void SnapToWall(Transform wallTransform)
    {
        if (planInstance == null || wallTransform == null) return;

        currentWallSnap = wallTransform;
        isSnappingToWall = true;

        BlueprintWallElement wallElement = wallTransform.GetComponent<BlueprintWallElement>();
        Vector3 localOffset = Vector3.zero;
        float yRotation = 0f;

        if (wallElement != null)
        {
            // Définir rotation et offset selon la direction
            switch (wallElement.direction)
            {
                case BlueprintWallElement.WallDirection.North:
                    yRotation = 0f;
                    localOffset = Vector3.zero; 
                    break;
                case BlueprintWallElement.WallDirection.East:
                    yRotation = 90f;
                    localOffset = new Vector3(0, 0, 2.5f); 
                    break;
                case BlueprintWallElement.WallDirection.South:
                    yRotation = 180f;
                    localOffset = new Vector3(0, 0, 2.5f); 
                    break;
                case BlueprintWallElement.WallDirection.West:
                    yRotation = -90f;
                    localOffset = Vector3.zero; 
                    break;
            }

            // Transformer l'offset local en espace monde
            Vector3 worldOffset = wallTransform.TransformDirection(localOffset);

            planInstance.transform.position = wallTransform.position + worldOffset;
        }
        else
        {
            // pas de wallElement : juste devant le mur
            planInstance.transform.position = wallTransform.position + wallTransform.forward;
            yRotation = wallTransform.eulerAngles.y;
        }

        planInstance.transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
        planInstance.SetActive(true);

        if (uiInstance != null)
            uiInstance.SetActive(false);
    }
    public virtual void Place()
    {
        isPlacing = false;

        RawMapRaycaster.instance.CleanActivePlaceable();

        if (uiInstance != null)
        {
        }

        if (planInstance != null)
        {
        }

        OnPlaced();
    }

    public void TryPlace()
    {
        if (!isPlacing)
            return;

        if (currentWallSnap.GetComponent<BlueprintWallElement>().groundLink == null)
        {
            Debug.Log("Impossible : ce mur n'appartient pas à votre zone achetée.");
            return;
        }

        if(currentWallSnap.GetComponent<BlueprintWallElement>().groundLink.GetComponent<BlueprintGroundElement>().nameShop != PanelShopMaster.instance.GetPanelShopSelected().GetShopName())
        {
            Debug.Log("Impossible : vous posez pas sur la zone correspondante.");
            return;
        }

        currentWallSnap.GetComponent<BlueprintWallElement>().HideWall();
        //TODO recup type of element 
        switch(typeOfElement)
        {
            case ElementType.Wall:
                break;
            case ElementType.Door:
                currentWallSnap.GetComponent<BlueprintWallElement>().CreateDoor();
                break;
            case ElementType.Window:
                currentWallSnap.GetComponent<BlueprintWallElement>().CreateWindow();
                break;
            case ElementType.ShopWindow:
                currentWallSnap.GetComponent<BlueprintWallElement>().CreateShopWindow();
                break;
        }
        

        Place();
    }

    protected virtual void OnPlaced()
    {
        // override dans les enfants si besoin
    }
}
