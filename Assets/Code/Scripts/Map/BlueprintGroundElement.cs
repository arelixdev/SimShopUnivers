using System.Collections.Generic;
using Mono.Cecil;
using UnityEngine;
using UnityEngine.AI;

public class BlueprintGroundElement : MonoBehaviour
{
    [HideInInspector]
    public Vector2Int gridIndex;
    private bool isSelected;
    public bool IsSelected => isSelected;

    [SerializeField] private GameObject objGround;

    [SerializeField] private Material matUnselected;
    [SerializeField] private Material matSelected;
    [SerializeField] private Material matBuy;


    public bool isBuy;
    public string nameShop;

    //TODO savoir quelle magasin l'a acheter peut etre utile pour eviter de faire n'importe quoi avec la pose des portes 

    private void Awake()
    {
        float cellSize = 2.5f;

        gridIndex = new Vector2Int(
            Mathf.RoundToInt(transform.position.x / cellSize),
            Mathf.RoundToInt(transform.position.z / cellSize)
        );
    }

    public void ToogleSelected()
    {
        isSelected = !isSelected;
        if(!isBuy)
        {
            if(isSelected)
            {
                objGround.GetComponent<MeshRenderer>().material = matSelected;
            } else
            {
                objGround.GetComponent<MeshRenderer>().material = matUnselected;
            }
        }
    }

    public void CleanGround()
    {
        objGround.GetComponent<MeshRenderer>().material = matUnselected;
        nameShop = null;
        isBuy = false;
        isSelected = false;
        
    }

    public void GroundBuy(string nsi)
    {
        isSelected = false;
        nameShop = nsi;
        isBuy = true;
        objGround.GetComponent<MeshRenderer>().material = matBuy;
    }

    public List<BlueprintGroundElement> GetNeighbors(Dictionary<Vector2Int, BlueprintGroundElement> grid)
    {
        List<BlueprintGroundElement> neighbors = new();

        Vector2Int[] dirs =
        {
            new Vector2Int(1,0),
            new Vector2Int(-1,0),
            new Vector2Int(0,1),
            new Vector2Int(0,-1),
        };

        foreach (var dir in dirs)
        {
            Vector2Int check = gridIndex + dir;
            if (grid.TryGetValue(check, out var neighbor))
            {
                if (neighbor != this)
                    neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    // Optionnel : world position
    public Vector3 WorldPosition(float cellSize = 2.5f)
    {
        return new Vector3(gridIndex.x * cellSize, 0, gridIndex.y * cellSize);
    }
}
