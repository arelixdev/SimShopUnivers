using UnityEngine;
using UnityEngine.InputSystem;

public class StoreController : MonoBehaviour
{
    public static StoreController instance;
    [SerializeField] private float currentMoney = 1000;
    [SerializeField] private Transform stockSpawnPoint;

    public Transform GetStockSpawnPoint()
    {
        return stockSpawnPoint;
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        UIController.instance.UpdateMoney(currentMoney);
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
}
