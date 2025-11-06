using System.Collections.Generic;
using UnityEngine;

public class StockBoxController : MonoBehaviour
{
    [SerializeField] private StockInfo info;

    public GameObject openBox, closeBox;

    [SerializeField] private List<Transform> boxPoints;
    [SerializeField] private List<Transform> drinkPoints;


    [SerializeField] private float moveSpeed = 5f;

    private List<StockObject> stockInBox = new List<StockObject>();

    public bool testFill;

    private Rigidbody rb;
    private Collider col;

    private bool isHeld;

    public int GetStockInBoxCount()
    {
        return stockInBox.Count;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    private void Start()
    {
        
    }

    private void Update() {
        if (testFill)
        {
            testFill = false;

            SetupBox(info);
        }
        
        if(isHeld)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, Vector3.zero, moveSpeed * Time.deltaTime);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.identity, moveSpeed * Time.deltaTime);
        }
    }

    public void SetupBox(StockInfo stockType)
    {
        info = stockType;

        List<Transform> activePoints = new List<Transform>();

        switch (info.typeOfStock)
        {
            case StockInfo.StockType.cereal:
                activePoints.AddRange(boxPoints);
                break;
            case StockInfo.StockType.drink:
                activePoints.AddRange(drinkPoints);
                break;
            case StockInfo.StockType.fruit:
                break;
        }

        if (stockInBox.Count == 0)
        {
            for (int i = 0; i < activePoints.Count; i++)
            {
                StockObject stock = Instantiate(stockType.stockObject, activePoints[i]);
                stock.transform.localPosition = Vector3.zero;
                stock.transform.localRotation = Quaternion.identity;

                stockInBox.Add(stock);

                stock.PlaceInBox();
            }
        }
    }

    public void Pickup()
    {
        rb.isKinematic = true;

        col.enabled = false;

        isHeld = true;
    }

    public void Release()
    {
        rb.isKinematic = false;
        col.enabled = true;
        isHeld = false;
    }

    public void OpenClose()
    {
        if (openBox.activeSelf)
        {
            openBox.SetActive(false);
            closeBox.SetActive(true);
            return;
        }

        if (closeBox.activeSelf)
        {
            openBox.SetActive(true);
            closeBox.SetActive(false);
        }
    }

    public void PlaceStockOnShelf(ShelfSpaceController shelf)
    {
        if (stockInBox.Count > 0)
        {
            shelf.PlaceStock(stockInBox[stockInBox.Count - 1]);

            if (stockInBox[stockInBox.Count - 1].GetIsPlaced())
            {
                stockInBox.RemoveAt(stockInBox.Count - 1);
            }
        }

        if (closeBox.activeSelf)
        {
            OpenClose();
        }
    }
    
    public int GetStockAmount(StockInfo.StockType type)
    {
        int toReturn = 0;

        switch(type)
        {
            case StockInfo.StockType.cereal:
                toReturn = boxPoints.Count;
                break;
            case StockInfo.StockType.drink:
                toReturn = drinkPoints.Count;
                break;
            case StockInfo.StockType.fruit:
                break;
        }
        return toReturn;
    }
}
