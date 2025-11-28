using System.Collections.Generic;
using NUnit.Framework.Interfaces;
using UnityEngine;

public class StockInfoController : MonoBehaviour
{
    public static StockInfoController instance;
    [SerializeField] private List<StockInfo> produceInfo;

    private List<StockInfo> allStock = new List<StockInfo>();
    private void Awake()
    {
        instance = this;

        allStock.AddRange(produceInfo);
        
        for (int i = 0; i < allStock.Count; i++)
        {
            if(allStock[i].currentPrice == 0)
            {
                allStock[i].currentPrice = allStock[i].price;
            }
        }
    }

    public StockInfo GetInfo(string stockName)
    {
        StockInfo infoToReturn = null;

        for (int i = 0; i < allStock.Count; i++)
        {
            if (stockName == allStock[i].name)
            {
                infoToReturn = allStock[i];
            }
        }

        return infoToReturn;
    }
    
    public void UpdatePrice(string stockName, float newPrice)
    {
        for (int i = 0; i < allStock.Count; i++)
        {
            if (stockName == allStock[i].name)
            {
                allStock[i].currentPrice = newPrice;
            }
        }

        List<ShelfSpaceController> shelves = new List<ShelfSpaceController>();

        shelves.AddRange(FindObjectsByType<ShelfSpaceController>(FindObjectsSortMode.None));

        foreach(ShelfSpaceController shelf in shelves)
        {
            if(shelf.info.name == stockName)
            {
                shelf.UpdateDisplayPrice(newPrice);
            }
        }
    }
}
