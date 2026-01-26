using System.Collections.Generic;
using UnityEngine;

public class BuyMenuController : MonoBehaviour
{
    [SerializeField] private GameObject stockPanel, furniturePanel;
    [SerializeField] private Transform contentStockPanel;
    [SerializeField] private GameObject buyStockFrame;

    public void OpenStockPanel()
    {
        stockPanel.SetActive(true);
        furniturePanel.SetActive(false);
    }
    
    public void OpenFurniturePanel()
    {
        stockPanel.SetActive(false);
        furniturePanel.SetActive(true);
    }

    public void InitStock()
    {
        for (int i = contentStockPanel.childCount - 1; i >= 0; i--)
        {
            Destroy(contentStockPanel.GetChild(i).gameObject);
        }

        List<StockInfoSO> allStock = StockInfoController.instance.GetAllStock();

        foreach (StockInfoSO info in allStock)
        {
            GameObject frame = Instantiate(buyStockFrame, contentStockPanel);
            BuyStockFrame stockFrame = frame.GetComponent<BuyStockFrame>();
            
            stockFrame.AddStockFrame(info);
        }
    }
}

public enum TypeShop
{
    cloth = 0,
    videoGame = 1,
    food = 2

}
