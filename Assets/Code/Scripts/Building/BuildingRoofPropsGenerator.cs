using UnityEngine;

public class BuildingRoofPropsGenerator : MonoBehaviour
{
    public GameObject[] roofPropsPrefab;

    [Range(0,100)] public int percentRoofPropsElement = 15;

    public void Generate()
    {
        GenerateProps();
    }

    void GenerateProps()
    {
        int rollFloor = Random.Range(0, 100);

        if (rollFloor <= percentRoofPropsElement && roofPropsPrefab.Length > 0)
        {
            SpawnFloorProp();
            return;
        }
    }

    void SpawnFloorProp()
    {
        if (roofPropsPrefab.Length == 0 )
            return;

        GameObject prefab = roofPropsPrefab[Random.Range(0, roofPropsPrefab.Length)];
        Instantiate(prefab, new Vector3(transform.position.x-4, transform.position.y, transform.position.z), Quaternion.identity, transform);
    }

}
