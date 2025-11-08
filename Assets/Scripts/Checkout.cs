using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Checkout : MonoBehaviour
{
    //TODO a changer dans le futur si on veut plusieurs checkout
    public static Checkout instance; 

    private void Awake() {
        instance = this;
    }

    [SerializeField] private TMP_Text priceText;
    [SerializeField] private GameObject checkoutScreen;

    [SerializeField] private Transform queuePoint;

    public List<CustomerController> customersInQueue = new List<CustomerController>();
    void Start()
    {
        HidePrice();
    }

    void Update()
    {
        if(customersInQueue.Count > 0 && !checkoutScreen.activeSelf)
        {
            if(Vector3.Distance(customersInQueue[0].transform.position, queuePoint.position) < 0.1f)
            {
                ShowPrice(customersInQueue[0].GetTotalSpend());
            }
        }
    }

    public void ShowPrice(float priceTotal)
    {
        checkoutScreen.SetActive(true);

        priceText.text = priceTotal.ToString("F2") + " €";
    }

    public void HidePrice()
    {
        checkoutScreen.SetActive(false);
    }

    public void CheckoutCustomer()
    {
        if(checkoutScreen.activeSelf && customersInQueue.Count > 0)
        {
            HidePrice();
            StoreController.instance.AddMoney(customersInQueue[0].GetTotalSpend());
            customersInQueue[0].StartLeaving();
            customersInQueue.RemoveAt(0);
            UpdateQueue();
        }
        
    }

    public void AddCustomerToQueue(CustomerController newCust)
    {
        customersInQueue.Add(newCust);
        UpdateQueue();
    }
    
    public void UpdateQueue()
    {
        for (int i = 0; i < customersInQueue.Count; i++)
        {
            customersInQueue[i].UpdateQueuePoint(queuePoint.position + (queuePoint.forward * i * 1.2f));
        }
    }
}
