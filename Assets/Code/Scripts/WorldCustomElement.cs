using UnityEngine;

public class WorldCustomElement : MonoBehaviour
{
    [SerializeField] private GameObject elementCreated;

    private bool isPainted;
    private GameObject element;

    public void PaintElement()
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
    }
}
