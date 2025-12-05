using System.Collections.Generic;
using Mono.Cecil;
using UnityEngine;
using UnityEngine.AI;

public class BlueprintGroundElement : MonoBehaviour
{
    public Vector3 gridPos;
    public bool isSelected;

    public List<BlueprintGroundElement> GetNeighbors(Dictionary<Vector3, BlueprintGroundElement> grid)
    {
        List<BlueprintGroundElement> result = new List<BlueprintGroundElement>();

        Vector3[] dirs =
        {
            Vector3.right * 2.5f, Vector3.left * 2.5f, Vector3.forward * 2.5f, Vector3.back * 2.5f
            //new Vector3(0,0,2.5f), new Vector3(0,0,-2.5f), new Vector3(2.5f,0,0), new Vector3(-2.5f,0,0)
        };

        foreach(var dir in dirs)
        {
            var check = gridPos + dir;
            if(grid.ContainsKey(check))
                result.Add(grid[check]);
        }

        return result;
    }
}
