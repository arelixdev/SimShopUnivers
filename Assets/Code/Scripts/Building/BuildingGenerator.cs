using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(BoxCollider))]
public class BuildingGenerator : MonoBehaviour
{
    public GameObject[] basePrefabs;
    public GameObject[] floorPrefabs;
    public GameObject[] roofPrefabs;
    public Material[] materials;

    public bool startCorner;
    public bool endCorner;

    public GameObject[] startBaseCorners;
    public GameObject[] startFloorCorners;
    public GameObject[] startRoofCorners;

    public GameObject[] endBaseCorners;
    public GameObject[] endFloorCorners;
    public GameObject[] endRoofCorners;

    public void Generate()
    {
        Clear();

        BoxCollider box = GetComponent<BoxCollider>();

        Vector3 center = box.center;
        Vector3 size = box.size;

        float startX = center.x - size.x * 0.5f;
        float endX   = center.x + size.x * 0.5f;
        float baseY  = center.y - size.y * 0.5f;
        float maxHeight = size.y;

        float currentX = startX;
        int buildingIndex = 0;

        while (currentX < endX)
        {
            bool isFirstBuilding = (buildingIndex == 0);

            // Pré-sélection normale (pour calculs)
            GameObject basePrefab  = basePrefabs[Random.Range(0, basePrefabs.Length)];
            GameObject floorPrefab = floorPrefabs[Random.Range(0, floorPrefabs.Length)];
            GameObject roofPrefab  = roofPrefabs[Random.Range(0, roofPrefabs.Length)];

            Bounds baseBounds  = GetLocalBounds(basePrefab);
            Bounds floorBounds = GetLocalBounds(floorPrefab);
            Bounds roofBounds  = GetLocalBounds(roofPrefab);

            if (currentX + baseBounds.size.x > endX)
                break;

            // Détection fiable du dernier
            bool isLastBuilding = true;
            foreach (GameObject prefab in basePrefabs)
            {
                Bounds b = GetLocalBounds(prefab);
                if (currentX + baseBounds.size.x + b.size.x <= endX)
                {
                    isLastBuilding = false;
                    break;
                }
            }

            // Remplacement par corners si nécessaire
            if (isFirstBuilding && startCorner)
            {
                basePrefab  = startBaseCorners[Random.Range(0, startBaseCorners.Length)];
                floorPrefab = startFloorCorners[Random.Range(0, startFloorCorners.Length)];
                roofPrefab  = startRoofCorners[Random.Range(0, startRoofCorners.Length)];

                baseBounds  = GetLocalBounds(basePrefab);
                floorBounds = GetLocalBounds(floorPrefab);
                roofBounds  = GetLocalBounds(roofPrefab);
            }
            else if (isLastBuilding && endCorner)
            {
                basePrefab  = endBaseCorners[Random.Range(0, endBaseCorners.Length)];
                floorPrefab = endFloorCorners[Random.Range(0, endFloorCorners.Length)];
                roofPrefab  = endRoofCorners[Random.Range(0, endRoofCorners.Length)];

                baseBounds  = GetLocalBounds(basePrefab);
                floorBounds = GetLocalBounds(floorPrefab);
                roofBounds  = GetLocalBounds(roofPrefab);
            }

            Vector3 basePos = new Vector3(
                currentX + baseBounds.size.x * 0.5f - baseBounds.center.x,
                baseBounds.center.y,
                center.z
            );

            GameObject buildingRoot =
                PrefabUtility.InstantiatePrefab(basePrefab) as GameObject;

            buildingRoot.transform.SetParent(transform);
            buildingRoot.transform.localPosition = basePos;

            

            float availableHeight = maxHeight - baseBounds.size.y - roofBounds.size.y;
            int floorCount = Mathf.FloorToInt(availableHeight / floorBounds.size.y);

            float currentY = baseBounds.size.y;

            for (int i = 0; i < floorCount; i++)
            {
                GameObject floor =
                    PrefabUtility.InstantiatePrefab(floorPrefab) as GameObject;

                floor.transform.SetParent(buildingRoot.transform);
                floor.transform.localPosition = Vector3.up * currentY;
                currentY += floorBounds.size.y;
            }

            GameObject roof =
                PrefabUtility.InstantiatePrefab(roofPrefab) as GameObject;

            roof.transform.SetParent(buildingRoot.transform);
            roof.transform.localPosition = Vector3.up * currentY;

            // Matériau unique par bâtiment
            if (materials.Length > 0)
            {
                Material mat = materials[Random.Range(0, materials.Length)];
                ApplyMaterialToBuilding(buildingRoot, mat);
            }

            currentX += baseBounds.size.x;
            buildingIndex++;
        }

        //FitBoxToBuildings();
    }

    public void Clear()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);
    }

    void ApplyMaterialToBuilding(GameObject root, Material mat)
    {
        Renderer[] renderers = root.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
            r.sharedMaterial = mat;
    }

    Bounds GetLocalBounds(GameObject prefab)
    {
        GameObject temp = Instantiate(prefab);
        temp.transform.position = Vector3.zero;
        temp.transform.rotation = Quaternion.identity;
        temp.transform.localScale = Vector3.one;

        Renderer[] renderers = temp.GetComponentsInChildren<Renderer>();

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer r in renderers)
            bounds.Encapsulate(r.bounds);

        bounds.center = temp.transform.InverseTransformPoint(bounds.center);

        DestroyImmediate(temp);
        return bounds;
    }

    void FitBoxToBuildings()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
            return;

        Bounds worldBounds = renderers[0].bounds;
        foreach (Renderer r in renderers)
            worldBounds.Encapsulate(r.bounds);

        Vector3 localSize = transform.InverseTransformVector(worldBounds.size);

        // On ne touche JAMAIS au center
        box.size = new Vector3(
            Mathf.Abs(localSize.x),
            box.size.y,
            Mathf.Abs(localSize.z)
        );
    }
}
