using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelShopMaster : MonoBehaviour
{
    public static PanelShopMaster instance;

    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject wallPrefabGame;
    [SerializeField] private Transform wallsParent;
    [SerializeField] private Transform wallsParentGame;
    [SerializeField] private float cellSize = 2.5f;
    [SerializeField] private GameObject mapPanelElement;
    [SerializeField] private GameObject linePanel;

    public Dictionary<Vector2Int, BlueprintGroundElement> grid = new();
    public List<BlueprintGroundElement> selectedElements = new();

    public bool customActivate;

    public List<BlueprintGroundElement> GetCurrentSelection()
    {
        return new List<BlueprintGroundElement>(selectedElements);
    }

    public void ClearSelection()
    {
        foreach (var e in selectedElements)
            e.ToogleSelected(); // visuel OFF

        selectedElements.Clear();
    }

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
        if (element.IsSelected)
        {
            RemoveFromSelection(element);
            return;
        }

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

    void RemoveFromSelection(BlueprintGroundElement element)
    {
        if (selectedElements.Contains(element))
        {
            selectedElements.Remove(element);
            element.ToogleSelected();

            // TODO : hide highlight if needed
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

    public bool CheckSelectionConnectivity()
    {
        if (selectedElements.Count <= 1)
            return true; // 1 élément = forcément connecté

        // BFS starting point
        Queue<BlueprintGroundElement> queue = new();
        HashSet<BlueprintGroundElement> visited = new();

        // On démarre depuis le premier élément sélectionné
        var start = selectedElements[0];
        queue.Enqueue(start);
        visited.Add(start);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            // Récupère voisins dans la grille
            foreach (var neigh in current.GetNeighbors(grid))
            {
                if (selectedElements.Contains(neigh) && !visited.Contains(neigh))
                {
                    visited.Add(neigh);
                    queue.Enqueue(neigh);
                }
            }
        }

        // Si tous les éléments visités = tous ceux sélectionnés → tout est connecté
        return visited.Count == selectedElements.Count;
    }

    public void BuildWallsAroundZone(List<BlueprintGroundElement> zone)
    {
        HashSet<BlueprintGroundElement> zoneSet = new(zone);

        foreach (var cell in zone)
        {
            // 4 directions
            Vector2Int[] dirs = new Vector2Int[]
            {
                new Vector2Int(1, 0),  // Est
                new Vector2Int(-1, 0), // Ouest
                new Vector2Int(0, 1),  // Nord
                new Vector2Int(0, -1), // Sud
            };

            foreach (var dir in dirs)
            {
                Vector2Int checkIndex = cell.gridIndex + dir;

                // On regarde si la case voisine existe dans la grille
                if (grid.TryGetValue(checkIndex, out BlueprintGroundElement neighbor))
                {
                    // Si le voisin ne fait PAS partie de la zone → on crée un mur entre les deux
                    if (!zoneSet.Contains(neighbor))
                    {
                        CreateWallBetween(cell, dir);
                    }
                }
            }
        }
    }

    private void CreateWallBetween(BlueprintGroundElement cell, Vector2Int dir)
    {
        Vector3 basePos = cell.WorldPosition(cellSize);

        Vector3 wallPos = Vector3.zero;
        Quaternion rot = Quaternion.identity;
        Quaternion rotGame = Quaternion.identity;

        // MUR EST (droite)
        if (dir.x == 1)
        {
            wallPos = new Vector3(
                basePos.x + cellSize,  
                basePos.y,
                basePos.z              
            );
            rot = Quaternion.Euler(0, -90, 0); 
            rotGame = Quaternion.Euler(0, 0, 0);
        }

        // MUR OUEST (gauche)
        else if (dir.x == -1)
        {
            wallPos = new Vector3(
                basePos.x,             
                basePos.y,
                basePos.z
            );
            rot = Quaternion.Euler(0, -90, 0); 
            rotGame = Quaternion.Euler(0, 0, 0);
        }

        // MUR NORD (haut)
        else if (dir.y == 1)
        {
            wallPos = new Vector3(
                basePos.x,
                basePos.y,
                basePos.z + cellSize   
            );
            rot = Quaternion.Euler(0, 0, 0); 
            rotGame = Quaternion.Euler(0, 90, 0);
        }

        // MUR SUD (bas)
        else if (dir.y == -1)
        {
            wallPos = new Vector3(
                basePos.x,
                basePos.y,
                basePos.z              
            );
            rot = Quaternion.Euler(0, 0, 0); 
            rotGame = Quaternion.Euler(0, 90, 0);
        }

        // 🔥 Correction universelle du décalage de ton prefab
        rot *= Quaternion.Euler(0, 90, 0);
        rotGame *= Quaternion.Euler(0, 90, 0);
        

        GameObject wall = Instantiate(wallPrefab, wallPos, rot, wallsParent);
        Instantiate(wallPrefabGame, wallPos, rotGame, wallsParentGame);
    }

    

}
