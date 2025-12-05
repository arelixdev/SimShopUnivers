using System.Collections.Generic;
using Mono.Cecil;
using UnityEngine;
using UnityEngine.AI;

public class BlueprintGroundElement : MonoBehaviour
{
    [HideInInspector]
    public Vector2Int gridIndex;
    private bool isSelected;

    [SerializeField] private GameObject objGround;

    [SerializeField] private Material matUnselected;
    [SerializeField] private Material matSelected;

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

        if(isSelected)
        {
            objGround.GetComponent<MeshRenderer>().material = matSelected;
        } else
        {
            objGround.GetComponent<MeshRenderer>().material = matUnselected;
        }
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
