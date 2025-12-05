using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelShopMaster : MonoBehaviour
{
    public static PanelShopMaster instance;
    [SerializeField] private GameObject mapPanelElement;
    [SerializeField] private GameObject linePanel;

    public Dictionary<Vector3, BlueprintGroundElement>  grid;
    public List<BlueprintGroundElement> selectedElements = new List<BlueprintGroundElement>();

    public bool customActivate;

    private void Awake() {
        instance = this;
        BuildGrid();
    }

    void BuildGrid()
    {
        grid.Clear();

        BlueprintGroundElement[] elements = FindObjectsOfType<BlueprintGroundElement>();

        foreach(var el in elements)
        {
            if(!grid.ContainsKey(el.gridPos))
            {
                grid.Add(el.gridPos, el);
            }
            else
            {
                Debug.LogWarning("Deux elements ont la meme position : " + el.gridPos + " " + el.name);
            }
        }
    }


    void Start()
    {
       CleanPanel(); 
    }

    public void CleanPanel()
    {
        for (int i = transform.childCount - 3; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        linePanel.SetActive(false);
    }

    public void AddShop()
    {
        GameObject newElement = Instantiate(mapPanelElement, transform);
        newElement.transform.SetSiblingIndex(transform.childCount - 3);

        if(!linePanel.activeSelf)
        {
            linePanel.SetActive(true);
        }

        StartCoroutine(RebuildNextFrame());
    }

    private IEnumerator RebuildNextFrame()
    {
        yield return new WaitForEndOfFrame();

        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    public void TrySelect(BlueprintGroundElement element)
    {
        if(selectedElements.Count == 0)
        {
            AddToSelection(element);
            return;
        }

        if(IsConnectedToSelection(element))
        {
            AddToSelection(element);
        } else
        {
            Debug.Log("Element non connecté -> impossible à selectionner");
        }
    }

    void AddToSelection(BlueprintGroundElement element)
    {
        if(!selectedElements.Contains(element))
        {
            selectedElements.Add(element);
            element.isSelected = true;

            //TODO SHOW HIGHLIGHT
        }
    }

    public bool IsConnectedToSelection(BlueprintGroundElement newElement)
    {
        HashSet<BlueprintGroundElement> visited = new HashSet<BlueprintGroundElement>();
        Queue<BlueprintGroundElement> queue = new Queue<BlueprintGroundElement>();

        foreach(var el in selectedElements)
            queue.Enqueue(el);

        while(queue.Count > 0)
        {
            var current = queue.Dequeue();
            if(current == newElement)
                return true;

            foreach(var n in current.GetNeighbors(grid))
            {
                if(!visited.Contains(n))
                {
                    visited.Add(n);
                    queue.Enqueue(n);
                }
            }
        }

        return false;
    }

}
