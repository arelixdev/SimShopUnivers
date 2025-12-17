using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelShopMaster : MonoBehaviour
{
    public static PanelShopMaster instance;

    public List<ShopElementDefinition> elementDatabase = new List<ShopElementDefinition>();
    public Transform mapMenuPanel;
    public Transform planParent;
    [SerializeField] private GameObject wallPrefab;
    public GameObject wallPrefabGame;
    public GameObject wallPrefabGameInteriorShop;
    [SerializeField] private Transform wallsParent;
    public Transform wallsParentGame;
    public float cellSize = 2.5f;
    [SerializeField] private GameObject mapPanelElement;
    [SerializeField] private GameObject linePanel;

    [SerializeField] private GameObject sellPanel;

    public Dictionary<Vector2Int, BlueprintGroundElement> grid = new Dictionary<Vector2Int, BlueprintGroundElement>();
    public Dictionary<WallKey, GameObject> createdWalls = new Dictionary<WallKey, GameObject>();
    public List<BlueprintGroundElement> selectedElements = new List<BlueprintGroundElement>();

    private PanelShopElement panelShopSelected;

    public bool customActivate;
    public bool deleteToolActive;

    public List<BlueprintGroundElement> lastBoughtZone;


    public List<BlueprintGroundElement> GetCurrentSelection()
    {
        return new List<BlueprintGroundElement>(selectedElements);
    }

    public ShopPlaceableElement GetElementById(string id)
    {
        foreach (var def in elementDatabase)
        {
            if (def.id == id)
                return def.prefab;
        }

        Debug.LogError("Element ID introuvable : " + id);
        return null;
    }

    public void DeleteInteriorWall(BlueprintWallElement wall)
    {
        var shop = GetPanelShopSelected();
        if (shop == null)
            return;

        // sécurité : seulement le shop actif
        if (wall.ownerShop != shop)
            return;

        // supprimer du dictionnaire global
        if (createdWalls.ContainsKey(wall.wallKey))
            createdWalls.Remove(wall.wallKey);

        // supprimer des listes du shop
        shop.allWallKeys.Remove(wall.wallKey);
        shop.allWallShop.Remove(wall.gameObject);

        if (wall.wallInGame != null)
        {
            shop.allWallGameShop.Remove(wall.wallInGame.gameObject);
            Destroy(wall.wallInGame.gameObject);
        }

        Destroy(wall.gameObject);
    }

    public PanelShopElement GetPanelShopSelected()
    {
        return panelShopSelected;
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
        sellPanel.SetActive(false);

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

    public void ShowSellPanel()
    {
        sellPanel.SetActive(true);
    }

    public void BtnYesSellPanel()
    {
        if(panelShopSelected != null)
        {
            panelShopSelected.ClearShop();
        }

        sellPanel.SetActive(false);
        Destroy(panelShopSelected.gameObject);
        if(transform.childCount <= 3)
        {
            linePanel.SetActive(false);
        }
    }

    public void CloseSellPanel()
    {
        sellPanel.SetActive(false);
    }

    public void AddElement(GameObject obj)
    {
        panelShopSelected.allElement.Add(obj);
    }

    

    public void AddShop()
    {
        GameObject newElement = Instantiate(mapPanelElement, transform);
        newElement.transform.SetSiblingIndex(transform.childCount - 3);

        ChangePanelSelected(newElement);

        if (!linePanel.activeSelf)
            linePanel.SetActive(true);

        StartCoroutine(RebuildNextFrame());
    }

    public void ChangePanelSelected(GameObject newElement)
    {
        if(panelShopSelected != null && panelShopSelected.gameObject != newElement && !panelShopSelected.GetIsRetracted())
        {
            panelShopSelected.TooglePanelShop();
        }

        panelShopSelected = newElement.GetComponent<PanelShopElement>();
    }

    public void RebuildShopMaster()
    {
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

    public void RemoveFromSelection(BlueprintGroundElement element)
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

    public void SetTooltipWallsColliders(bool state)
    {
        MapTooltipsElement[] tooltips = FindObjectsOfType<MapTooltipsElement>();

        foreach (var t in tooltips)
        {
            if (t.wallElement != null)
            {
                Collider col = t.wallElement.GetComponent<Collider>();
                if (col != null)
                    col.enabled = state;
            }
        }
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

    public void BuildWallsAroundZone(List<BlueprintGroundElement> zone, PanelShopElement shop)
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

                if (grid.TryGetValue(checkIndex, out BlueprintGroundElement neighbor))
                {
                    if (!zoneSet.Contains(neighbor))
                    {
                        CreateWallBetween(cell, dir, shop);
                    }
                }
            }
        }
    }

    private void CreateWallBetween(BlueprintGroundElement cell, Vector2Int dir, PanelShopElement shop)
    {
        WallKey key = new WallKey(cell.gridIndex, dir);

        if (createdWalls.ContainsKey(key))
            return;

        Vector3 basePos = cell.WorldPosition(cellSize);

        Vector3 wallPos = Vector3.zero;
        Quaternion rot = Quaternion.identity;
        Quaternion rotGame = Quaternion.identity;

        // MUR EST 
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

        // MUR OUEST 
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

        // MUR NORD 
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

        // MUR SUD 
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

        rot *= Quaternion.Euler(0, 90, 0);
        rotGame *= Quaternion.Euler(0, 90, 0);
        

        GameObject wall = Instantiate(wallPrefab, wallPos, rot, wallsParent);
        GameObject wallGame = Instantiate(wallPrefabGame, wallPos, rotGame, wallsParentGame);

        if (shop != null)
        {
            shop.allWallShop.Add(wall);
            shop.allWallGameShop.Add(wallGame);
            shop.allWallKeys.Add(key);
        }

        createdWalls.Add(key, wall);
    }

    public void RebuildAllShopWalls()
    {
        // Nettoyage total
        foreach (var wall in createdWalls.Values)
        {
            if (wall != null)
                Destroy(wall);
        }

        createdWalls.Clear();

        // Rebuild pour chaque shop actif
        PanelShopElement[] shops = FindObjectsOfType<PanelShopElement>();

        foreach (var shop in shops)
        {
            if (!shop.HasBoughtZone())
                continue;

            BuildWallsAroundZone(shop.GetGroundElements(), shop);
        }
    }

    public bool IsWallTouchingZone(Transform wallTransform)
{
    BlueprintWallElement wall = wallTransform.GetComponent<BlueprintWallElement>();
    if (wall == null)
        return false;

    Vector3 wp = wallTransform.position;
    float size = 2.5f;  // 2.5
    float eps = 0.05f;

    foreach (var zone in lastBoughtZone)
    {
        Vector3 zp = zone.transform.position;

        float westX  = zp.x;
        float eastX  = zp.x + size;
        float southZ = zp.z;
        float northZ = zp.z + size;

        switch (wall.direction)
        {
            case Direc.West:
                // pivot = (tile.x, tile.z)
                if (Mathf.Abs(wp.x - westX) < eps &&
                    wp.z >= southZ - eps && wp.z < northZ + eps)
                    return true;
                break;

            case Direc.East:
                // pivot = (tile.x + size, tile.z)
                if (Mathf.Abs(wp.x - eastX) < eps &&
                    wp.z >= southZ - eps && wp.z < northZ + eps)
                    return true;
                break;

            case Direc.South:
                // pivot = (tile.x, tile.z)
                if (Mathf.Abs(wp.z - southZ) < eps &&
                    wp.x >= westX - eps && wp.x < eastX + eps)
                    return true;
                break;

            case Direc.North:
                // pivot = (tile.x, tile.z + size)
                if (Mathf.Abs(wp.z - northZ) < eps &&
                    wp.x >= westX - eps && wp.x < eastX + eps)
                    return true;
                break;
        }
    }

    return false;
}

}

public struct WallKey
{
    public Vector2Int a;
    public Vector2Int b;

    public WallKey(Vector2Int cell, Vector2Int dir)
    {
        a = cell;
        b = cell + dir;

        // On normalise : toujours a < b
        if (a.x > b.x || (a.x == b.x && a.y > b.y))
        {
            (a, b) = (b, a);
        }
    }

    public override bool Equals(object obj)
    {
        if (obj is WallKey other)
            return a.Equals(other.a) && b.Equals(other.b);
        return false;
    }

    public override int GetHashCode()
    {
        return a.GetHashCode() ^ (b.GetHashCode() << 1);
    }
}

[System.Serializable]
public class ShopElementDefinition
{
    public string id;                      // ex: "door", "table"
    public ShopPlaceableElement prefab;    // le prefab contenant ton script
}
