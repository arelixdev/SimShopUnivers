using System;
using System.Collections.Generic;
using UnityEngine;

public class StockInfoController : MonoBehaviour
{
    public static StockInfoController instance;
    [SerializeField] private List<StockInfoSO> produceInfo;
    public List<ListElementInShop> elementInShop;

    public List<ShopTypeSO> allShopType;

    public List<ElementSO> allDoors = new List<ElementSO>();

    private List<StockInfoSO> allStock = new List<StockInfoSO>();


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

    public List<StockInfoSO> GetAllStock()
    {
        return allStock;
    }

    public StockInfoSO GetInfo(string stockName)
    {
        StockInfoSO infoToReturn = null;

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

[Serializable]
public class ListElementInShop
{
    public TypeShop typeShop;
    public List<StockInfoSO> elementInShop = new();
}
