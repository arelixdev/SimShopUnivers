using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShopZone : MonoBehaviour
{
    [SerializeField] private string nameShop;
    [SerializeField] private TextMeshPro nameObj;
    [SerializeField] private float textYOffset = 0.5f; 
    [SerializeField] private Color color = Color.yellow;
    private Material mat;
    private SphereCollider sphere;

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
        //nameObj.text = nameShop.ToUpper();
    }


     void Awake()
    {
        sphere = GetComponent<SphereCollider>();
        mat = new Material(Shader.Find("Hidden/Internal-Colored"));

        if (nameObj != null)
            nameObj.text = nameShop.ToUpper();; 
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
        UIController.instance.UpdateShopUI(nameShop, levelShop, xpAct);
    }

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Player")
        {
            UIController.instance.UpdateShopUI(nameShop, levelShop, xpAct);
            playerIn = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            UIController.instance.UpdateShopUI("", 0, 0);
            playerIn = false;
        }
    }
}
