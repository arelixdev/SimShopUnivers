using System;
using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private Transform objectPoint;

    [SerializeField] private Camera checkoutCamera;

    public List<CustomerController> customersInQueue = new List<CustomerController>();
    public List<StockObject> objectsInQueue = new List<StockObject>();

    [Header("UI Checkout")]
    [SerializeField] private Transform contentZone;
    [SerializeField] private GameObject layoutElementScan;
    void Start()
    {
        for (int i = contentZone.childCount - 1; i >= 0; i--)
        {
            Destroy(contentZone.GetChild(i).gameObject);
        }
    }

    void Update()
    {
        if(customersInQueue.Count > 0 && !checkoutScreen.activeSelf)
        {
            CustomerController first = customersInQueue[0];

            if(Vector3.Distance(customersInQueue[0].transform.position, queuePoint.position) < 0.1f)
            {
                if (first.HasNotTransferredObjectsYet)
                {
                    TransferCustomerObjects(first);
                }

                //ShowPrice(customersInQueue[0].GetTotalSpend());
            }
        }
    }

    private void TransferCustomerObjects(CustomerController customer)
    {
        AddObjectToQueue(customer.GetStockInBag());
        customer.MarkObjectsAsTransferred();
    }



    public void CheckoutCustomer()
    {
        if(checkoutScreen.activeSelf && customersInQueue.Count > 0)
        {
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

    public void AddObjectToQueue(List<StockObject> objs)
    {
        foreach (StockObject obj in objs)
        {
            obj.transform.SetParent(objectPoint);
            obj.transform.localScale = Vector3.one;

            obj.MarkAsCheckoutItem();
        }
        objectsInQueue.AddRange(objs);
        UpdateObjectsQueue(); 
    }

    public void RemoveObjectFromQueue(StockObject obj)
    {
        if (objectsInQueue.Contains(obj))
            objectsInQueue.Remove(obj);
    }

    public void UpdateObjectsQueue()
    {
        for (int i = 0; i < objectsInQueue.Count; i++)
        {
            StockObject obj = objectsInQueue[i];

            // SECURITÉ → au cas où un script interne remettrait un scale foireux
            obj.transform.localScale = Vector3.one;

            Vector3 newPos = objectPoint.position + (objectPoint.forward * i * 0.5f);

            obj.transform.position = newPos;
            obj.transform.rotation = objectPoint.rotation;

            SetLayerRecursively(obj.gameObject, LayerMask.NameToLayer("CheckoutStock"));
        }
    }

    public void UpdateScreen(StockObject obj)
    {
        ScanLineCheckout line = Instantiate(layoutElementScan, contentZone).GetComponent<ScanLineCheckout>();
        line.UpdateLine(obj.info);

        LayoutRebuilder.ForceRebuildLayoutImmediate(contentZone as RectTransform);
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    public void ActiveCam()
    {
        checkoutCamera.gameObject.SetActive(true);
    }

    internal void DesactivateCam()
    {
        checkoutCamera.gameObject.SetActive(false);
    }

    public void CloseCheckout()
    {
        PlayerController.instance.CloseCheckout();
    }
}
