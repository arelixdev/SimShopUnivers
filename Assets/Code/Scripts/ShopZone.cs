using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ShopZone : MonoBehaviour
{
    public Transform centerPoint;
    [SerializeField] private string nameShop;
    [SerializeField] private TypeShop shopType;
    [SerializeField] private Image icnShop;

    private int levelShop = 1;
    private int xpAct = 0;

    public List<FurnitureController> shelvingsInZone = new List<FurnitureController>();

    public string GetNameShop()
    {
        return nameShop;
    }

    public void SetNameShop(string nameMod)
    {
        nameShop = nameMod;
    }

    public void SetTypeShop(int typeValue)
    {
        shopType = (TypeShop)typeValue;
    }

    void Update()
    {
        /*if(playerIn && Keyboard.current.pKey.wasPressedThisFrame)
        {
            AddXp(60);
        }*/
    }

    void AddXp(int val)
    {
        xpAct += val;


        if(xpAct >= StoreController.instance.GetXpRequiered()[levelShop-1])
        {
            int diffVal = xpAct - StoreController.instance.GetXpRequiered()[levelShop-1];
            levelShop++;
            xpAct = diffVal;
        }
        UIController.instance.UpdateShopUI(nameShop, levelShop, xpAct, shopType);
    }

    public void RegisterFurniture(FurnitureController furniture)
    {
        if (!shelvingsInZone.Contains(furniture))
        {
            shelvingsInZone.Add(furniture);
        }
    }

    public void UnregisterFurniture(FurnitureController furniture)
    {
        shelvingsInZone.Remove(furniture);
    }

    public bool ContainsFurniture(FurnitureController furniture)
    {
        return shelvingsInZone.Contains(furniture);
    }

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Player")
        {
            UIController.instance.UpdateShopUI(nameShop, levelShop, xpAct, shopType);
        }

        FurnitureController furniture = other.GetComponent<FurnitureController>();
        if (furniture != null)
        {
            furniture.SetCurrentShopZone(this);
            RegisterFurniture(furniture);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            UIController.instance.UpdateShopUI("", 0, 0, shopType);
        }
    }
}
