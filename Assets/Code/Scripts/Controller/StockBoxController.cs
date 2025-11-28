using System.Collections.Generic;
using UnityEngine;

public class StockBoxController : MonoBehaviour
{
    [SerializeField] private StockInfoSO info;

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

    public void SetupBox(StockInfoSO stockType)
    {
        info = stockType;

        List<Transform> activePoints = new List<Transform>();

        switch (info.typeOfStock)
        {
            case StockInfoSO.StockType.cereal:
                activePoints.AddRange(boxPoints);
                break;
            case StockInfoSO.StockType.drink:
                activePoints.AddRange(drinkPoints);
                break;
            case StockInfoSO.StockType.fruit:
                break;
        }

        if (stockInBox.Count == 0)
        {
            for (int i = 0; i < activePoints.Count; i++)
            {
                StockObject stock = Instantiate(stockType.stockObject, activePoints[i]);
                stock.info = info;
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
        if (stockInBox.Count > 0 && shelf != null)
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
    
    public int GetStockAmount(StockInfoSO.StockType type)
    {
        int toReturn = 0;

        switch(type)
        {
            case StockInfoSO.StockType.cereal:
                toReturn = boxPoints.Count;
                break;
            case StockInfoSO.StockType.drink:
                toReturn = drinkPoints.Count;
                break;
            case StockInfoSO.StockType.fruit:
                break;
        }
        return toReturn;
    }
}
