using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShelfSpaceController : MonoBehaviour
{
    public StockInfo info;
    public List<StockObject> objectsOnShelf;

    [SerializeField] private List<Transform> boxPoints;
    [SerializeField] private List<Transform> drinkPoints;

    [SerializeField] private TMP_Text shelfLabel;

    private void Awake() {
        shelfLabel.text = string.Empty; 
    }


    public void PlaceStock(StockObject objectToPlace)
    {
        bool preventPlacing = true;

        if (objectsOnShelf.Count == 0)
        {
            info = objectToPlace.info;
            preventPlacing = false;
        }
        else
        {
            if (info.name == objectToPlace.info.name)
            {
                preventPlacing = false;

                switch(info.typeOfStock)
                {
                    case StockInfo.StockType.cereal:
                        if(objectsOnShelf.Count >= boxPoints.Count)
                        {
                            preventPlacing = true;
                        }
                        break;
                    case StockInfo.StockType.drink:
                        if(objectsOnShelf.Count >= drinkPoints.Count)
                        {
                            preventPlacing = true;
                        }
                        break;
                    case StockInfo.StockType.fruit:
                        if(objectsOnShelf.Count >= drinkPoints.Count)
                        {
                            preventPlacing = true;
                        }
                        break;
                }

                
            }
        }

        if (!preventPlacing)
        {
            //objectToPlace.transform.SetParent(transform);
            objectToPlace.MakePlace();

            switch(info.typeOfStock)
            {
                case StockInfo.StockType.cereal:
                    objectToPlace.transform.SetParent(boxPoints[objectsOnShelf.Count]);
                    break;
                case StockInfo.StockType.drink:
                    objectToPlace.transform.SetParent(drinkPoints[objectsOnShelf.Count]);
                    break;
                case StockInfo.StockType.fruit:
                    /*if(objectsOnShelf.Count >= drinkPoints.Count)
                    {
                        preventPlacing = true;
                    }*/
                    break;
            }



            objectsOnShelf.Add(objectToPlace);

            UpdateDisplayPrice(info.currentPrice);
        }
    }

    public StockObject GetStock()
    {
        StockObject objectToReturn = null;

        if (objectsOnShelf.Count > 0)
        {
            objectToReturn = objectsOnShelf[objectsOnShelf.Count - 1];

            objectsOnShelf.RemoveAt(objectsOnShelf.Count - 1);
        }

        if (objectsOnShelf.Count == 0)
        {
            shelfLabel.text = string.Empty;
        }

        return objectToReturn;
    }

    public void StartPriceUpdate()
    {
        if (objectsOnShelf.Count > 0)
        {
            UIController.instance.OpenUpdatePrice(info);
        }
    }
    
    public void UpdateDisplayPrice(float price)
    {
        if(objectsOnShelf.Count > 0)
        {
            info.currentPrice = price;

            shelfLabel.text = info.currentPrice.ToString("F2") + " €";
        }
        
    }
}
