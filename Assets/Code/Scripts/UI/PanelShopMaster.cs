using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelShopMaster : MonoBehaviour
{
    public static PanelShopMaster instance;
    [SerializeField] private GameObject mapPanelElement;
    [SerializeField] private GameObject linePanel;

    public Dictionary<Vector2Int, BlueprintGroundElement> grid = new();
    public List<BlueprintGroundElement> selectedElements = new();

    public bool customActivate;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        CleanPanel();
        BuildGrid();
    }

    void BuildGrid()
    {
        grid.Clear();
        BlueprintGroundElement[] elements = FindObjectsOfType<BlueprintGroundElement>();

        foreach (var el in elements)
        {
            if (!grid.ContainsKey(el.gridIndex))
                grid.Add(el.gridIndex, el);
            else
                Debug.LogWarning("Duplicate grid pos: " + el.gridIndex + " " + el.name);
        }

    }

    public void CleanPanel()
    {
        for (int i = transform.childCount - 3; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);

        linePanel.SetActive(false);
    }

    public void AddShop()
    {
        GameObject newElement = Instantiate(mapPanelElement, transform);
        newElement.transform.SetSiblingIndex(transform.childCount - 3);

        if (!linePanel.activeSelf)
            linePanel.SetActive(true);

        StartCoroutine(RebuildNextFrame());
    }

    private IEnumerator RebuildNextFrame()
    {
        yield return new WaitForEndOfFrame();

        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    public void TrySelect(BlueprintGroundElement element)
    {
        if (selectedElements.Count == 0)
        {
            AddToSelection(element);
            return;
        }

        if (IsConnectedToSelection(element))
            AddToSelection(element);
        else
            Debug.Log("Element non connecté -> impossible à selectionner");
    }

    void AddToSelection(BlueprintGroundElement element)
    {
        if (!selectedElements.Contains(element))
        {
            selectedElements.Add(element);
            element.ToogleSelected();

            // TODO : show highlight
        }
    }

    bool IsConnectedToSelection(BlueprintGroundElement newElement)
    {
        foreach (var neighbor in newElement.GetNeighbors(grid))
        {
            if (selectedElements.Contains(neighbor))
                return true; // il touche au moins un élément déjà sélectionné
        }

        return false;
    }

}
