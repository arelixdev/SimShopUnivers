using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShelfSpaceController : MonoBehaviour
{
    public StockInfoSO info;
    public List<StockObject> objectsOnShelf = new ();

    [SerializeField] private List<Transform> boxPoints;
    [SerializeField] private List<Transform> drinkPoints;

    [SerializeField] private TMP_Text shelfLabel;

    private void Awake() {
        shelfLabel.text = string.Empty; 
    }

    public bool HasStock()
    {
        return info != null && objectsOnShelf.Count > 0;
    }


    public void PlaceStock(StockObject objectToPlace)
    {
        if (objectsOnShelf == null)
            objectsOnShelf = new List<StockObject>();

        bool preventPlacing = true;

        if (objectsOnShelf.Count == 0)
        {
            info = objectToPlace.info;
            preventPlacing = false;
        }
        else
        {
            if (info != null && info.name == objectToPlace.info.name)
            {
                preventPlacing = false;

                switch (info.typeOfStock)
                {
                    case StockType.cereal:
                        if (objectsOnShelf.Count >= boxPoints.Count)
                            preventPlacing = true;
                        break;

                    case StockType.drink:
                    case StockType.fruit:
                        if (objectsOnShelf.Count >= drinkPoints.Count)
                            preventPlacing = true;
                        break;
                }
            }
        }

        if (!preventPlacing)
        {
            objectToPlace.MakePlace();

            switch (info.typeOfStock)
            {
                case StockType.cereal:
                    objectToPlace.transform.SetParent(boxPoints[objectsOnShelf.Count]);
                    break;

                case StockType.drink:
                case StockType.fruit:
                    objectToPlace.transform.SetParent(drinkPoints[objectsOnShelf.Count]);
                    break;
            }

            objectToPlace.transform.localPosition = Vector3.zero;
            objectToPlace.transform.localRotation = Quaternion.identity;

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
            info = null;
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
