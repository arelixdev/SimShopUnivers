using UnityEngine;

public class BuyMenuController : MonoBehaviour
{
    [SerializeField] private GameObject stockPanel, furniturePanel;

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
}
