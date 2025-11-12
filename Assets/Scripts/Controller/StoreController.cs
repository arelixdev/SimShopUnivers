using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class StoreController : MonoBehaviour
{
    public static StoreController instance;
    [SerializeField] private float currentMoney = 1000;
    [SerializeField] private Transform stockSpawnPoint, furnitureSpawnPoint;

    public List<FurnitureController> shelvingCases = new List<FurnitureController>();

    private bool isOpen;

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
