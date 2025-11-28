using UnityEngine;

public class StockObject : MonoBehaviour
{
    public StockInfo info;
    [SerializeField] private float moveSpeed;

    private bool isPlaced;
    private Rigidbody rb;
    private Collider col;

    private bool inBag;

    private bool isAtCheckout = false;

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
        if (isAtCheckout)
            return;

        if (isPlaced)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, Vector3.zero, moveSpeed * Time.deltaTime);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.identity, moveSpeed * Time.deltaTime);
        }
        
        if(inBag)
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.zero, Time.deltaTime);
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

    public void PlaceInBag()
    {
        inBag = true;
        MakePlace();
    }

    public void MarkAsCheckoutItem()
    {
        isAtCheckout = true;
        isPlaced = false;
        inBag = false;

        rb.isKinematic = true;
        col.enabled = true;
    }

    public void OutCheckout()
    {
        isAtCheckout = false;

        rb.isKinematic = false;
        col.enabled = false;
    }
}
