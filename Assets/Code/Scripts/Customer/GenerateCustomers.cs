using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class GenerateCustomers : MonoBehaviour
{
    public List<Material> mat;
    private void Start() {
        Generate();
    }

    [Button("Generate")]
    public void Generate()
    {
        int childCount = transform.childCount;

        for (int i = 1; i < childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        int rand = Random.Range(1, childCount);

        transform.GetChild(rand).gameObject.SetActive(true);

        int matRand = Random.Range(0, mat.Count);

        transform.GetChild(rand).GetComponent<SkinnedMeshRenderer>().material = mat[matRand];
    }
}
