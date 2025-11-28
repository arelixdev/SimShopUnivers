using System.Collections.Generic;
using UnityEngine;

public class FurnitureController : MonoBehaviour
{
    [SerializeField] private GameObject mainObject, placementObject;
    [SerializeField] private Transform standPoint;
    public List<ShelfSpaceController> shelves = new List<ShelfSpaceController>();

    public float price;

    private Collider col;

    public Transform GetStandPoint()
    {
        return standPoint;
    }
    

    private void Awake()
    {
        col = GetComponent<Collider>();
    }

    private void Start()
    {
        
        if(shelves.Count > 0)
        {
            StoreController.instance.shelvingCases.Add(this);
        }
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
}
