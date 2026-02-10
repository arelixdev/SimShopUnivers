using System;
using UnityEngine;

public class WallInMallElement : MonoBehaviour
{
    [SerializeField] private GameObject wallDownObject;
    [SerializeField] private Transform doorPivot;

    public GameObject elementBlueprintOnWall;

    public WorldCustomElement downElement;

    void Start()
    {
        if (wallDownObject != null)
        {
            downElement = wallDownObject.GetComponent<WorldCustomElement>();
        }

        HideOtherElement();
    }

    public void ShowDoor()
    {
        if (downElement == null) return;

        downElement.listWalls[0].SetActive(false);
        downElement.listWalls[1].SetActive(true);
        downElement.listWalls[2].SetActive(false);
        downElement.listWalls[3].SetActive(false);
        downElement.elementType = ElementType.Door;
    }

    public void ShowWindow()
    {
        downElement.listWalls[0].SetActive(false);
        downElement.listWalls[1].SetActive(false);
        downElement.listWalls[2].SetActive(true);
        downElement.listWalls[3].SetActive(false);
        downElement.elementType = ElementType.Window;
    }
    public void ShowShopWindow()
    {
        downElement.listWalls[0].SetActive(false);
        downElement.listWalls[1].SetActive(false);
        downElement.listWalls[2].SetActive(false);
        downElement.listWalls[3].SetActive(true);
        downElement.elementType = ElementType.ShopWindow;
    }

    public void HideOtherElement()
    {
        if (downElement == null) return;

        downElement.listWalls[0].SetActive(true);
        downElement.listWalls[1].SetActive(false);
        downElement.listWalls[2].SetActive(false);
        downElement.listWalls[3].SetActive(false);
        downElement.elementType = ElementType.Wall;
    }

    internal void CreateDoor(int val)
    {
        foreach (Transform child in doorPivot) {
            Destroy(child.gameObject);
        }
        GameObject door = Instantiate(StockInfoController.instance.allDoors[val].elementPrefab, doorPivot);
        PanelShopMaster.instance.AddElement(door);

    }
}
