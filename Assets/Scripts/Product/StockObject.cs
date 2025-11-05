using UnityEngine;

public class StockObject : MonoBehaviour
{
    public StockInfo info;
    [SerializeField] private float moveSpeed;

    private bool isPlaced;
    private Rigidbody rb;
    private Collider col;

    public bool GetIsPlaced()
    {
        return isPlaced;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }
    
    private void Start()
    {
        info = StockInfoController.instance.GetInfo(info.name); 
    }
    
    private void Update() {
        if(isPlaced)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, Vector3.zero, moveSpeed * Time.deltaTime);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.identity, moveSpeed * Time.deltaTime);
        }
    }

    public void Pickup()
    {
        rb.isKinematic = true;

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        isPlaced = false;
        col.enabled = false;
    }

    public void MakePlace()
    {
        rb.isKinematic = true;
        isPlaced = true;
        col.enabled = false;
    }

    public void Release()
    {
        rb.isKinematic = false;
        col.enabled = true;
    }

    public void PlaceInBox()
    {
        rb.isKinematic = true;
        col.enabled = false;
    }
}
