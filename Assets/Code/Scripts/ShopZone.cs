using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ShopZone : MonoBehaviour
{
    public Transform centerPoint;
    public MeshCollider meshCollider;
    [SerializeField] private string nameShop;
    [SerializeField] private TypeShop shopType;
    [SerializeField] private Image icnShop;

    private int levelShop = 1;
    private int xpAct = 0;

    public List<FurnitureController> shelvingsInZone = new List<FurnitureController>();
    public List<Checkout> checkoutsInZone = new();

    public bool HasCheckout()
    {
        return checkoutsInZone != null && checkoutsInZone.Count > 0;
    }

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

    public Vector3 GetRandomPointInside()
    {
        Bounds b = meshCollider.bounds;

        for (int i = 0; i < 10; i++) // plusieurs tentatives
        {
            Vector3 randomPoint = new Vector3(
                Random.Range(b.min.x, b.max.x),
                b.center.y,
                Random.Range(b.min.z, b.max.z)
            );

            if (meshCollider.bounds.Contains(randomPoint))
                return randomPoint;
        }

        // fallback sécurité
        return b.center;
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
        switch (furniture.GetFurnitureType())
        {
            case FurnitureType.Checkout:
                Checkout checkout = furniture as Checkout;
                if (checkout != null && !checkoutsInZone.Contains(checkout))
                    checkoutsInZone.Add(checkout);
                break;

            case FurnitureType.Shelf:
                if (!shelvingsInZone.Contains(furniture))
                    shelvingsInZone.Add(furniture);
                break;
        }
    }

    public void UnregisterFurniture(FurnitureController furniture)
    {
        switch (furniture.GetFurnitureType())
        {
            case FurnitureType.Checkout:
                checkoutsInZone.Remove(furniture as Checkout);
                break;

            case FurnitureType.Shelf:
                shelvingsInZone.Remove(furniture);
                break;
        }
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
