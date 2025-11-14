using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CustomerController : MonoBehaviour
{
    [SerializeField] private List<NavPoint> points = new List<NavPoint>();

    [SerializeField] private float moveSpeed;

    [SerializeField] private Animator animator;

    public enum CustomerState
    {
        entering, browsing, queuing, atCheckout, leaving
    }

    [SerializeField] private CustomerState currentState;
    [SerializeField] private int maxBrowsePoints = 5;
    private int browsePointsRemain;

    [SerializeField] private float browseTime;
    [SerializeField] private FurnitureController currentShelfCase;

    [SerializeField] private GameObject shoppingBag;

    [SerializeField] private float waitAfterGrabbing = 0.5f;

    private List<StockObject> stockInBag = new List<StockObject>();

    private float currentWaitTime;
    private bool hasGrabbed;

    private Vector3 queuePoint;

    private NavMeshAgent agent;

    private bool objectsTransferred = false;

    public bool HasNotTransferredObjectsYet => !objectsTransferred;

    public List<StockObject> GetStockInBag()
    {
        return stockInBag;
    }

    public void MarkObjectsAsTransferred()
    {
        objectsTransferred = true;
    }

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        points.Clear();
        points.AddRange(CustomersManager.instance.GetEntryPoints());

        if (points.Count > 0)
        {
            transform.position = points[0].point.position;

            currentWaitTime = points[0].waitTime;
        }

        //points.AddRange(CustomersManager.instance.GetExitPoints());
    }

    void Update()
    {
        switch(currentState)
        {
            case CustomerState.entering:
                if (points.Count > 0)
                {
                    MoveToPoint();
                } else
                {
                    if(StoreController.instance.GetIsOpen())
                    {
                        currentState = CustomerState.browsing;

                        browsePointsRemain = UnityEngine.Random.Range(1, maxBrowsePoints + 1);
                        browsePointsRemain = Mathf.Clamp(browsePointsRemain, 1, StoreController.instance.shelvingCases.Count);

                        GetBrowsePoint();
                    } else
                    {
                        StartLeaving();
                    }
                    
                }
                break;
            case CustomerState.browsing:
                MoveToPoint();

                if (points.Count == 0)
                {
                    if(!hasGrabbed)
                    {
                        GrabStock();
                    } else
                    {
                        hasGrabbed = false;

                        browsePointsRemain--;
                        if(browsePointsRemain > 0)
                        {
                            GetBrowsePoint();
                        } else
                        {
                            if(stockInBag.Count > 0)
                            {
                                Checkout.instance.AddCustomerToQueue(this);

                                currentState = CustomerState.queuing;
                            } else
                            {
                                StartLeaving();
                            }
                        }
                    }
                }
                break;
            case CustomerState.queuing:
                transform.position = Vector3.MoveTowards(transform.position, queuePoint, moveSpeed * Time.deltaTime);
                
                if(Vector3.Distance(transform.position, queuePoint) > .1f)
                {
                    animator.SetBool("IsMoving", true);
                } else
                {
                    animator.SetBool("IsMoving", false);
                }

                break;
            case CustomerState.atCheckout:
                break;
            case CustomerState.leaving:
                if (points.Count > 0)
                {
                    MoveToPoint();
                } else
                {
                    Destroy(gameObject);
                }
                break;
        }
    }

    public void MoveToPoint()
    {
        if (points.Count == 0) return;

        Vector3 targetPosition = new Vector3(points[0].point.position.x, transform.position.y, points[0].point.position.z);
        agent.SetDestination(targetPosition);
        animator.SetBool("IsMoving", agent.velocity.magnitude > 0.1f);

        // Vérifier si le client est arrivé
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.isStopped = true;
            currentWaitTime -= Time.deltaTime;

            if (currentWaitTime <= 0)
                StartNextPoint();
        }
        else
        {
            agent.isStopped = false;
        }
    }

    public void StartNextPoint()
    {
        if (points.Count > 0) points.RemoveAt(0);

        if (points.Count > 0)
        {
            currentWaitTime = points[0].waitTime;
            agent.isStopped = false;
        }
        else if (currentState == CustomerState.leaving)
        {
            Destroy(gameObject);
        }
    }

    public void StartLeaving()
    {
        currentState = CustomerState.leaving;

        points.Clear();
        points.AddRange(CustomersManager.instance.GetExitPoints());
    }

    void GetBrowsePoint()
    {
        points.Clear();

        int selectedShelf = UnityEngine.Random.Range(0, StoreController.instance.shelvingCases.Count);

        points.Add(new NavPoint());
        points[0].point = StoreController.instance.shelvingCases[selectedShelf].GetStandPoint();

        points[0].waitTime = browseTime * UnityEngine.Random.Range(0.75f, 1.25f);

        currentWaitTime = points[0].waitTime;

        currentShelfCase = StoreController.instance.shelvingCases[selectedShelf];
    }

    public void GrabStock()
    {

        hasGrabbed = true;

        int shelf = UnityEngine.Random.Range(0, currentShelfCase.shelves.Count);

        StockObject stock = currentShelfCase.shelves[shelf].GetStock();

        if (stock != null)
        {
            stock.transform.SetParent(shoppingBag.transform);
            stockInBag.Add(stock);
            stock.PlaceInBag();

            shoppingBag.SetActive(true);

            points.Clear();
            points.Add(new NavPoint());
            points[0].point = currentShelfCase.GetStandPoint();
            points[0].waitTime = waitAfterGrabbing * UnityEngine.Random.Range(0.75f, 1.25f);
            currentWaitTime = points[0].waitTime;
        }
    }

    public void GrabCheckout(StockObject obj)
    {
        obj.transform.SetParent(shoppingBag.transform);
        stockInBag.Add(obj);
        obj.PlaceInBag();
}

    public void UpdateQueuePoint(Vector3 newPoint)
    {
        queuePoint = newPoint;
        transform.LookAt(queuePoint);
    }
    
    public float GetTotalSpend()
    {
        float total = 0;


        foreach(StockObject stock in stockInBag)
        {
            total += stock.info.currentPrice;
        }


        return total;
    }

    public void AddObjectToBag(StockObject obj)
    {
        obj.PlaceInBag();
    }
}

[Serializable]
public class NavPoint
{
    public Transform point;
    public float waitTime;
}
