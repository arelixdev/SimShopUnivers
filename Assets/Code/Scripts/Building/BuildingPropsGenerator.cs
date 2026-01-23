using UnityEngine;
using System.Collections.Generic;

public class BuildingPropsGenerator : MonoBehaviour
{
    public GameObject[] floorPropsElement;
    public GameObject[] windowPropsElement;

    [Range(0,100)] public int percentFloorPropsElement = 15;

    [Header("Windows distribution (must total 100)")]
    [Range(0,100)] public int percentNothingWindowPropsElement = 35;
    [Range(0,100)] public int percentOneWindowPropsElement = 30;
    [Range(0,100)] public int percentTwoWindowPropsElement = 25;
    [Range(0,100)] public int percentThreeWindowPropsElement = 10;

    public Transform clotheslinePoint;
    public Transform[] windowsPoint;

    public void Generate()
    {
        GenerateProps();
    }

    void GenerateProps()
    {
        int rollFloor = Random.Range(0, 100);

        if (rollFloor <= percentFloorPropsElement && floorPropsElement.Length > 0)
        {
            SpawnFloorProp();
            return;
        }

        GenerateWindowProps();
    }

    void SpawnFloorProp()
    {
        if (floorPropsElement.Length == 0 || clotheslinePoint == null)
            return;

        GameObject prefab = floorPropsElement[Random.Range(0, floorPropsElement.Length)];
        Instantiate(prefab, clotheslinePoint.position, clotheslinePoint.rotation, transform);
    }

    void GenerateWindowProps()
    {
        if (windowPropsElement.Length == 0 || windowsPoint.Length == 0)
            return;

        int roll = Random.Range(0, 100);

        int nothingMax = percentNothingWindowPropsElement;
        int oneMax     = nothingMax + percentOneWindowPropsElement;
        int twoMax     = oneMax     + percentTwoWindowPropsElement;
        int threeMax   = twoMax     + percentThreeWindowPropsElement;

        int count = 0;

        if (roll <= nothingMax)
            count = 0;
        else if (roll <= oneMax)
            count = 1;
        else if (roll <= twoMax)
            count = 2;
        else if (roll <= threeMax)
            count = 3;

        SpawnOnRandomWindowPoints(count);
    }

    void SpawnOnRandomWindowPoints(int count)
    {
        if (count <= 0)
            return;
            
        count = Mathf.Min(count, windowsPoint.Length);

        List<int> availableIndexes = new List<int>();
        for (int i = 0; i < windowsPoint.Length; i++)
            availableIndexes.Add(i);

        for (int i = 0; i < count && availableIndexes.Count > 0; i++)
        {
            int randIndex = Random.Range(0, availableIndexes.Count);
            int windowIndex = availableIndexes[randIndex];
            availableIndexes.RemoveAt(randIndex);

            Transform point = windowsPoint[windowIndex];
            GameObject prefab = windowPropsElement[Random.Range(0, windowPropsElement.Length)];

            Instantiate(prefab, point.position, point.rotation, transform);
        }
    }
}
