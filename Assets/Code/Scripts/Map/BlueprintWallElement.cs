using UnityEngine;

public class BlueprintWallElement : MonoBehaviour
{
    public enum WallDirection
    {
        North,
        East,
        South,
        West
    }

    public WallDirection direction = WallDirection.North;

    [SerializeField] public WallInMallElement wallInGame;

    [HideInInspector]
    public GameObject groundLink;

    [SerializeField] private GameObject wallAppearance;

    private void OnTriggerStay(Collider other) {
        BlueprintGroundElement groundElement = other.GetComponent<BlueprintGroundElement>();
        if(groundElement != null && groundElement.isBuy && groundLink == null)
        {
            groundLink = groundElement.gameObject;
        }
    }

    public void HideWall()
    {
        wallAppearance.SetActive(false);
    }

    public void ShowWall()
    {
        wallAppearance.SetActive(true);
    }

    public void CreateDoor()
    {
        wallInGame.ShowDoor();
    }

    public void CreateWindow()
    {
        wallInGame.ShowWindow();
    }

    
}


