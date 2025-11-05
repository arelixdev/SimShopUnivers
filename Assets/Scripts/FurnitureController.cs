using UnityEngine;

public class FurnitureController : MonoBehaviour
{
    [SerializeField] private GameObject mainObject, placementObject;

    private Collider col;

    private void Awake()
    {
        col = GetComponent<Collider>();
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
