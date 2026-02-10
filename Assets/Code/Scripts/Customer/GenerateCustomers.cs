using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class GenerateCustomers : MonoBehaviour
{
    public List<Material> mat;

    public Transform accesHeadTransform;
    public Transform accesMouthTransform;
    public Transform accesEarTransform;
    public Transform accesEyesTransform;
    public int percentHaveAccesHead;
    public int percentHaveAccesMouth;
    public int percentHaveAccesEar;
    public int percentHaveAccesEyes;
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

        if(accesHeadTransform != null)
        {
            rand = 0;
            for (int i = 0; i < accesHeadTransform.childCount; i++)
            {
                accesHeadTransform.GetChild(i).gameObject.SetActive(false);
            }

            int randPercent = Random.Range(0, 100);

            if(randPercent <= percentHaveAccesHead)
            {
                rand = Random.Range(0, accesHeadTransform.childCount);

                accesHeadTransform.GetChild(rand).gameObject.SetActive(true);
            }

            matRand = Random.Range(0, mat.Count);

            accesHeadTransform.GetChild(rand).GetComponent<MeshRenderer>().material = mat[matRand];
        }

        if(accesEarTransform != null)
        {
            rand = 0;
            for (int i = 0; i < accesEarTransform.childCount; i++)
            {
                accesEarTransform.GetChild(i).gameObject.SetActive(false);
            }

            int randPercent = Random.Range(0, 100);

            if(randPercent <= percentHaveAccesEar)
            {
                rand = Random.Range(0, accesEarTransform.childCount);

                accesEarTransform.GetChild(rand).gameObject.SetActive(true);
            }
            matRand = Random.Range(0, mat.Count);

            accesEarTransform.GetChild(rand).GetComponent<MeshRenderer>().material = mat[matRand];
        }

        if(accesEyesTransform != null)
        {
            rand = 0;
            for (int i = 0; i < accesEyesTransform.childCount; i++)
            {
                accesEyesTransform.GetChild(i).gameObject.SetActive(false);
            }

            int randPercent = Random.Range(0, 100);

            if(randPercent <= percentHaveAccesEyes)
            {
                rand = Random.Range(0, accesEyesTransform.childCount);

                accesEyesTransform.GetChild(rand).gameObject.SetActive(true);
            }
            matRand = Random.Range(0, mat.Count);

            accesEyesTransform.GetChild(rand).GetComponent<MeshRenderer>().material = mat[matRand];
        }

        if(accesMouthTransform != null)
        {
            rand = 0;
            for (int i = 0; i < accesMouthTransform.childCount; i++)
            {
                accesMouthTransform.GetChild(i).gameObject.SetActive(false);
            }

            int randPercent = Random.Range(0, 100);

            if(randPercent <= percentHaveAccesMouth)
            {
                rand = Random.Range(0, accesMouthTransform.childCount);

                accesMouthTransform.GetChild(rand).gameObject.SetActive(true);
            }

            matRand = Random.Range(0, mat.Count);

            accesEyesTransform.GetChild(rand).GetComponent<MeshRenderer>().material = mat[matRand];
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
