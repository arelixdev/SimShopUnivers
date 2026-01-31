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
    private bool playerIn;

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

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Player")
        {
            UIController.instance.UpdateShopUI(nameShop, levelShop, xpAct, shopType);
            playerIn = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            UIController.instance.UpdateShopUI("", 0, 0, shopType);
            playerIn = false;
        }
    }
}
