using UnityEngine;

public class WallInMallElement : MonoBehaviour
{
    [SerializeField] private GameObject wallDownObject;

    private WorldCustomElement downElement;

    void Start()
    {
        if (wallDownObject != null)
        {
            downElement = wallDownObject.GetComponent<WorldCustomElement>();
        }

        HideDoor();
    }

    public void ShowDoor()
    {
        if (downElement == null) return;

        downElement.listWalls[0].SetActive(false);
        downElement.listWalls[1].SetActive(true);
        downElement.elementType = ElementType.Door;
    }

    public void HideDoor()
    {
        if (downElement == null) return;

        downElement.listWalls[0].SetActive(true);
        downElement.listWalls[1].SetActive(false);
        downElement.elementType = ElementType.Wall;
    }
}
