using System.Collections.Generic;
using UnityEngine;

public enum FurnitureType
{
    Shelf,
    Checkout
}

public class FurnitureController : MonoBehaviour
{
    [SerializeField] protected FurnitureType furnitureType;
    [SerializeField] private GameObject mainObject, placementObject;
    [SerializeField] private Transform standPoint;
    public List<ShelfSpaceController> shelves = new List<ShelfSpaceController>();

    public FurnitureType GetFurnitureType() => furnitureType;

    public float price;

    private ShopZone currentShopZone;

    private Collider col;

    public Transform GetStandPoint()
    {
        return standPoint;
    }
    

    protected virtual void Awake()
    {
        col = GetComponent<Collider>();
    }

    private void Start()
    {
        
        if(shelves != null && shelves.Count > 0)
        {
            StoreController.instance.shelvingCases.Add(this);
        }
    }

    public virtual bool CanBeMoved()
    {
        return true;
    }

    public void MakePlaceable()
    {
        mainObject.SetActive(false);
        placementObject.SetActive(true);

        col.enabled = false;
    }
    
    public void PlaceFurniture()
    {
        mainObject.SetActive(true);
        placementObject.SetActive(false);

        col.enabled = true;
    }

    public void SetCurrentShopZone(ShopZone zone)
    {
        currentShopZone = zone;
    }

    public void ClearShopZone()
    {
        currentShopZone = null;
    }

    public void LeaveShopZone()
    {
        if (currentShopZone != null)
        {
            currentShopZone.UnregisterFurniture(this);
            currentShopZone = null;
        }
    }
}
