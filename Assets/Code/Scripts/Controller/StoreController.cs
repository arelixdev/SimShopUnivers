using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class StoreController : MonoBehaviour
{
    public static StoreController instance;
    [SerializeField] private float currentMoney = 1000;
    [SerializeField] private Transform stockSpawnPoint, furnitureSpawnPoint;
    [SerializeField] private List<int> levelXpRequiered = new List<int>(); //TODO rename var

    //TODO add serializeField for clean
    public int levelGeneral;
    public List<int> levelXpGeneral = new List<int>();

    public int xpAct;

    public List<FurnitureController> shelvingCases = new List<FurnitureController>();

    private bool isOpen;

    public List<int> GetXpRequiered()
    {
        return levelXpRequiered;
    }

    public bool GetIsOpen()
    {
        return isOpen;
    }

    public Transform GetStockSpawnPoint()
    {
        return stockSpawnPoint;
    }

    public Transform GetFurnitureSpawnPoint()
    {
        return furnitureSpawnPoint;
    }

    public void OpenStore()
    {
        isOpen = true;
        NavMeshcontroller.instance.OpenShopUpdate();
        TimeController.instance.isRunning = true;
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        UIController.instance.UpdateMoney(currentMoney);

        TimeController.instance.OnTimeFinished += EndOfDay;

        UIController.instance.UpdateXpGeneralUI(levelGeneral, xpAct);
    }

    void Update()
    {
        if (Keyboard.current.iKey.wasPressedThisFrame)
        {
            AddMoney(100);
        }
        
        if (Keyboard.current.oKey.wasPressedThisFrame)
        {
            if(CheckMoneyAvailable(300))
            {
                SpendMoney(300);
            }
            
        }

        if(Keyboard.current.pKey.wasPressedThisFrame)
        {
            AddXp(60);
        }
    }

    void AddXp(int val)
    {
        xpAct += val;


        if(xpAct >= levelXpGeneral[levelGeneral-1])
        {
            int diffVal = xpAct - levelXpGeneral[levelGeneral-1];
            levelGeneral++;
            xpAct = diffVal;
        }
        UIController.instance.UpdateXpGeneralUI( levelGeneral, xpAct);
    }

    public void AddMoney(float amountToAdd)
    {
        currentMoney += amountToAdd;

        UIController.instance.UpdateMoney(currentMoney);
    }

    public void SpendMoney(float amountToSpend)
    {
        currentMoney -= amountToSpend;

        if (currentMoney < 0)
        {
            currentMoney = 0;
        }

        UIController.instance.UpdateMoney(currentMoney);
    }

    public bool CheckMoneyAvailable(float amountToCheck)
    {
        bool hasEnough = false;

        if (currentMoney >= amountToCheck)
        {
            hasEnough = true;
        }

        return hasEnough;
    }
    
    void EndOfDay()
    {
        isOpen = false;
        Debug.Log("La journée est terminée !");
    }
}
