using UnityEngine;

public class WorldCustomElement : MonoBehaviour
{
    [SerializeField] private GameObject elementCreated;

    private bool isPainted;
    private GameObject element;

    public void PaintElement(Material mat, int val)
    {
        if(!isPainted)
        {
            isPainted = true;

            if (transform.childCount > 0)
            {
                Destroy(transform.GetChild(0).gameObject);
            }

            element = Instantiate(elementCreated, transform);
            element.transform.localPosition = Vector3.zero;
            element.transform.localRotation = Quaternion.identity;

        }

        if(val == -1 || val == 0)
        {
            element.GetComponent<MeshRenderer>().material = mat;
        } else if (val == 1)
        {
            MeshRenderer rend = element.GetComponent<MeshRenderer>();
            Material[] mats = rend.materials;
            mats[1] = mat;
            rend.materials = mats;
        }

        
    }
}
