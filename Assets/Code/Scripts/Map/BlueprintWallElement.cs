using UnityEngine;
public enum Direc
    {
        North,
        East,
        South,
        West
    }

public class BlueprintWallElement : MonoBehaviour
{
    

    public Direc direction = Direc.North;

    [SerializeField] public WallInMallElement wallInGame;
    public GameObject groundLink;

    public GameObject wallAppearance;

    private void OnTriggerStay(Collider other) {
        BlueprintGroundElement groundElement = other.GetComponent<BlueprintGroundElement>();
        if(groundElement != null && groundElement.isBuy && groundLink == null)
        {
            groundLink = groundElement.gameObject;
        }
    }

    public void HideWall()
    {
        GetComponent<BoxCollider>().enabled = false;
        HideDisplayWall();
    }

    public void HideDisplayWall()
    {
        wallAppearance.SetActive(false);
    }

    public void ShowWall()
    {

        GetComponent<BoxCollider>().enabled = true;
    }

    public void ShowDisplayWall()
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

    public void CreateShopWindow()
    {
        wallInGame.ShowShopWindow();
    }

    
}


