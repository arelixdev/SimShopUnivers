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

    public WallInMallElement wallInGame;
    public GameObject groundLink;

    public GameObject wallAppearance;

    public bool isInteriorWall;
    public PanelShopElement ownerShop;
    public WallKey wallKey;

    private void OnTriggerStay(Collider other) {
        if (groundLink != null)
        return;

        if (other.TryGetComponent(out BlueprintGroundElement groundElement))
        {
            if (groundElement.isBuy)
            {
                groundLink = groundElement.gameObject;
            }
        }
    }

    public void ComputeDirectionFromRotation()
    {
        // forward du mur
        Vector3 fwd = transform.forward;

        // On regarde quel axe est dominant
        if (Mathf.Abs(fwd.z) > Mathf.Abs(fwd.x))
        {
            // Mur vertical (Nord/Sud)
            direction = Direc.East;
        }
        else
        {
            // Mur horizontal (Est/Ouest)
            direction = Direc.North;
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
        ShowDisplayWall();
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


