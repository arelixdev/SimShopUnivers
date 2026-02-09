using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class GenerateCustomers : MonoBehaviour
{
    public List<Material> mat;

    public Transform accessoryTransform;
    public int percentHaveAccessory;
    public Transform hairTransform;
    public int percentHaveHair;

    public Transform beardTransform;
    public int percentHaveBeard;
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

        if(accessoryTransform != null)
        {
            for (int i = 0; i < accessoryTransform.childCount; i++)
            {
                accessoryTransform.GetChild(i).gameObject.SetActive(false);
            }

            int randPercent = Random.Range(0, 100);

            if(randPercent <= percentHaveAccessory)
            {
                rand = Random.Range(0, accessoryTransform.childCount);

                accessoryTransform.GetChild(rand).gameObject.SetActive(true);
            }
        }

        if(hairTransform != null)
        {
            for (int i = 0; i < hairTransform.childCount; i++)
            {
                hairTransform.GetChild(i).gameObject.SetActive(false);
            }

            int randPercent = Random.Range(0, 100);

            if(randPercent <= percentHaveHair)
            {
                rand = Random.Range(0, hairTransform.childCount);

                hairTransform.GetChild(rand).gameObject.SetActive(true);

                //Change mat
                matRand = Random.Range(0, mat.Count);

                hairTransform.GetChild(rand).GetComponent<MeshRenderer>().material = mat[matRand];
            }
        }

        if(beardTransform != null)
        {
            for (int i = 0; i < beardTransform.childCount; i++)
            {
                beardTransform.GetChild(i).gameObject.SetActive(false);
            }

            int randPercent = Random.Range(0, 100);

            if(randPercent <= percentHaveBeard)
            {
                rand = Random.Range(0, beardTransform.childCount);

                beardTransform.GetChild(rand).gameObject.SetActive(true);

                beardTransform.GetChild(rand).GetComponent<MeshRenderer>().material = mat[matRand];
            }
        }
    }
}
