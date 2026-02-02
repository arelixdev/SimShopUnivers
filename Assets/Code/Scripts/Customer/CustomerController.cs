using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CustomerController : MonoBehaviour
{
    [SerializeField] private List<NavPoint> points = new List<NavPoint>();

    [SerializeField] private float moveSpeed;

    [SerializeField] private Animator animator;

    [Header("Shop Need")]
    [SerializeField]
    public List<ShopList> shopList;

    private int numberShop;

    [SerializeField] private int numberShopMin = 1;
    [SerializeField] private int numberShopMax = 3;

    [Header("Requirements")]
    public Need foodNeed;
    public Need peeNeed;
    public Need comfortNeed;
    public Need energyNeed;
    public Need distractionNeed;

    public Satisfy satisfaction;

    public enum CustomerState
    {
        entering, browsing, need, queuing, atCheckout, leaving
    }

    [SerializeField] public CustomerState currentState;

    [SerializeField] private GameObject shoppingBag;

    [SerializeField] private float waitAfterGrabbing = 0.5f;

    [Header("Trash Customer")]

    [SerializeField] private GameObject waterTrash;
    [SerializeField] private float minTrashInterval = 8f;
    [SerializeField] private float maxTrashInterval = 15f;

    [SerializeField] private float trashSpawnChance = 0.3f; 

    public List<NeedType> needList;

    private NeedType currentNeed;
    private bool isSolvingNeed;

    private NeedInteractionPoint currentInteractionPoint;

    private float nextTrashTime = 0f;

    private List<StockObject> stockInBag = new List<StockObject>();

    private float currentWaitTime;
    private bool hasGrabbed;

    private Vector3 queuePoint;

    private NavMeshAgent agent;

    private bool objectsTransferred = false;

    public bool HasNotTransferredObjectsYet => !objectsTransferred;

    private bool payWithCard;

    private bool leave;

    private bool hasReachedCurrentPoint = false;

    private void OnEnable()
    {
        StoreController.OnStoreOpened += OnStoreOpened;
        StoreController.OnStoreClosed += OnStoreClosed;
    }

    private void OnDisable()
    {
        StoreController.OnStoreOpened -= OnStoreOpened;
        StoreController.OnStoreClosed -= OnStoreClosed;
    }

    public List<StockObject> GetStockInBag()
    {
        return stockInBag;
    }

    public void MarkObjectsAsTransferred()
    {
        objectsTransferred = true;
    }

    public bool GetPayWithCard()
    {
        return payWithCard;
    }

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        int randVal = UnityEngine.Random.Range(0,100);
        if(randVal >= 50)
        {
            payWithCard = true;
        }

        points.Clear();

        NavPoint randomPointClose = new NavPoint
        {
            point = CustomersManager.instance.randomPointClose[UnityEngine.Random.Range(0, CustomersManager.instance.randomPointClose.Count)].point,
            waitTime = 1.5f
        };

        points.Add(randomPointClose);
        

        if (points.Count > 0)
        {
            currentWaitTime = points[0].waitTime;
        }

        nextTrashTime = Time.time + UnityEngine.Random.Range(minTrashInterval, maxTrashInterval);

        RequierementInit();

        numberShop = UnityEngine.Random.Range(numberShopMin, numberShopMax);
        

        int randNum = UnityEngine.Random.Range(1,4);

        for (int i = 0; i < randNum; i++)
        {
            TypeShop[] sl = (TypeShop[])Enum.GetValues(typeof(TypeShop));
            TypeShop selectRandomType = (TypeShop)UnityEngine.Random.Range(0, sl.Length);
            GenerateShopList(selectRandomType);
        }

        
    }

    private void GenerateShopList(TypeShop selectRandomType)
    {
        ShopList newShopList = new ShopList
        {
            typeShop = selectRandomType,
            listStockType = new List<StockInfoSO>()
        };

        List<StockInfoSO> availableStocks = new List<StockInfoSO>();

        foreach (var elem in StockInfoController.instance.elementInShop)
        {
            if (elem.typeShop == selectRandomType && elem.elementInShop.Count > 0)
            {
                availableStocks.AddRange(elem.elementInShop);
            }
        }

        if (availableStocks.Count == 0)
        {
            Debug.LogWarning("Aucun stock disponible pour le shop : " + selectRandomType);
            shopList.Add(newShopList);
            return;
        }

        int realNumberShop = Mathf.Min(numberShop, availableStocks.Count);

        for (int i = 0; i < realNumberShop; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, availableStocks.Count);

            StockInfoSO selectedStock = availableStocks[randomIndex];

            newShopList.listStockType.Add(selectedStock);

            availableStocks.RemoveAt(randomIndex);
        }

        shopList.Add(newShopList);
    }

    private void RequierementInit()
    {
        foodNeed.Init(this);
        peeNeed.Init(this);
        comfortNeed.Init(this);
        distractionNeed.Init(this);
        energyNeed.Init(this);

        satisfaction.Init(this);
    }

    void Update()
    {
        HandleTrashSpawn();
        
        if(!leave)
        {
            /*if (!isSolvingNeed && needList.Count > 0)
            {
                StartSolvingNeed();
            }
            NeedsUpdate();*/
        }
        

        switch(currentState)
        {
            //Entrancce
            case CustomerState.entering:
                if (points.Count > 0)
                {
                    MoveToPoint();
                } else
                {
                    /*if(StoreController.instance.GetIsOpen() && StoreController.instance.shelvingCases.Count > 0)
                    {
                        currentState = CustomerState.browsing;
                    } else
                    {
                        StartLeaving();
                    }*/
                }
                break;
            //Take objects
            case CustomerState.browsing:
                MoveToPoint();
                if (points.Count == 0)
                {
                    
                } else
                {
                    CheckArrivalAtPoint();
                }
                break;
            //Go make what u need 
            case CustomerState.need:
                    MoveToPoint();
                    
                break;
            //Go to buy 
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
            //At Checkout 
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

    private void CheckArrivalAtPoint()
    {
        if (hasReachedCurrentPoint)
            return;

        if (agent.pathPending)
            return;

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            if (agent.velocity.sqrMagnitude == 0f)
            {
                hasReachedCurrentPoint = true;

                Debug.Log($"[{name}] est arrivé à son point de browsing " + $"({points[0].GetPosition()})");

                // 👉 ici ton système de waitTime continue de fonctionner
            }
        }
    }

    private void StartSolvingNeed()
    {
        if (needList.Count == 0)
        {
            Debug.Log($"{name} est mécontent (aucun besoin)");
            return;
        }

        currentState = CustomerState.need;
        isSolvingNeed = true;

        int attempts = needList.Count;

        while (attempts > 0)
        {
            currentNeed = needList[0];

            currentInteractionPoint = FindClosestSatisfyPoint(currentNeed);

            if (currentInteractionPoint != null)
            {
                currentInteractionPoint.ChangeOccupation(true);

                points.Clear();
                points.Add(new NavPoint
                {
                    point = currentInteractionPoint.standPoint,
                    waitTime = 1f
                });

                currentWaitTime = points[0].waitTime;

                agent.ResetPath();
                agent.SetDestination(points[0].point.position);

                return; 
            }

            needList.RemoveAt(0);
            needList.Add(currentNeed);

            attempts--;
        }

        if(needList.Count > 0 && isSolvingNeed)
        {
            NeedType stock = needList[0];

            needList.RemoveAt(0);
            needList.Add(stock);

            satisfaction.Decrease(1);
        }

        isSolvingNeed = false;
        
        //TODO change to go back to activity currentState => "Last state"
        StartLeaving();

        Debug.Log($"{name} est mécontent (tous les points sont occupés)");
    }
    private Need GetNeed(NeedType type)
    {
        return type switch
        {
            NeedType.Food => foodNeed,
            NeedType.Pee => peeNeed,
            NeedType.Comfort => comfortNeed,
            NeedType.Energy => energyNeed,
            NeedType.Distraction => distractionNeed,
            _ => null
        };
    }

    private NeedInteractionPoint FindClosestSatisfyPoint(NeedType need)
    {
        NeedInteractionPoint[] points = FindObjectsByType<NeedInteractionPoint>(FindObjectsSortMode.None);


        float minDist = float.MaxValue;
        NeedInteractionPoint closest = null;

        foreach (var p in points)
        {
            if (p.needType != need) continue;
            if (p.isOccuped) continue;

            float d = Vector3.Distance(transform.position, p.transform.position);
            if (d < minDist)
            {
                minDist = d;
                closest = p;
            }
        }

        return closest;
    }

    private void NeedsUpdate()
    {
        foodNeed.Decrease(gameObject);
        peeNeed.Decrease(gameObject);
        comfortNeed.Decrease(gameObject);
        distractionNeed.Decrease(gameObject);
        energyNeed.Decrease(gameObject);
    }

    private void HandleTrashSpawn()
    {
        if (waterTrash == null) return;

        if (Time.time >= nextTrashTime)
        {
            float rand = UnityEngine.Random.Range(0f, 1f);

            if (rand <= trashSpawnChance)
            {
                Vector3 rayOrigin = transform.position + Vector3.up * 1f;

                if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 5f))
                {
                    Vector3 spawnPos = hit.point;

                    GameObject trash = Instantiate(waterTrash, spawnPos, Quaternion.identity);

                    float randomY = UnityEngine.Random.Range(0f, 360f);
                    trash.transform.rotation = Quaternion.Euler(0, randomY, 0);

                    float randomScale = UnityEngine.Random.Range(0.8f, 1.2f);
                    trash.transform.localScale = Vector3.one * randomScale;
                }
                else
                {
                    Vector3 fallbackPos = transform.position + new Vector3(0, -0.05f, 0);

                    GameObject trash = Instantiate(waterTrash, fallbackPos, Quaternion.identity);
                }
            }

            nextTrashTime = Time.time + UnityEngine.Random.Range(minTrashInterval, maxTrashInterval);
        }
    }

    public void MoveToPoint()
    {
        if (points.Count == 0) return;

        Vector3 targetPosition = points[0].GetPosition();
        targetPosition.y = transform.position.y;

        if (agent.isOnNavMesh)
        {
            agent.SetDestination(targetPosition);
            
        }
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
        if (points.Count > 0)
            points.RemoveAt(0);

        hasReachedCurrentPoint = false;

        if (points.Count > 0)
        {
            currentWaitTime = points[0].waitTime;
            agent.isStopped = false;
        }
        else
        {
            if (currentState == CustomerState.need)
            {
                ResolveNeed();
            }
            else if (currentState == CustomerState.leaving)
            {
                Destroy(gameObject);
            }
        }
    }

    private void ResolveNeed()
    {
        Need need = GetNeed(currentNeed);
        if (need != null && currentInteractionPoint != null)
        {
            need.Increase(currentInteractionPoint.needAmountValue);
        }

        // Supprimer le need traité de la liste
        needList.Remove(currentNeed);


        currentInteractionPoint.ChangeOccupation(false);

        currentInteractionPoint = null;
        isSolvingNeed = false;

        // S'il reste des besoins -> on passe au suivant
        if (needList.Count > 0)
        {
            StartSolvingNeed();
        }
        else
        {
            currentState = CustomerState.browsing;
        }
    }

    public void StartLeaving()
    {
        currentState = CustomerState.leaving;

        leave = true;

        points.Clear();

        NavPoint exitPoint = new NavPoint
        {
            point = CustomersManager.instance.allSpawnPoint[UnityEngine.Random.Range(0, CustomersManager.instance.allSpawnPoint.Count)],
            waitTime = 1.5f
        };

        points.Add(exitPoint);
    }



    public void GrabCheckout(StockObject obj)
    {
        obj.transform.SetParent(shoppingBag.transform);
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

    private void OnStoreOpened()
    {
        if (PanelShopMaster.instance == null)
        {
            Debug.LogWarning("PanelShopMaster introuvable");
            return;
        }

        List<ShopCreated> createdShops = PanelShopMaster.instance.listShopCreated;

        if (createdShops == null || createdShops.Count == 0)
        {
            //Debug.Log($"{name} : aucun shop n'est créé dans le magasin");
            return;
        }

        bool hasMatchingShop = false;

        foreach (ShopList wantedShop in shopList)
        {
            foreach (ShopCreated createdShop in createdShops)
            {
                if (wantedShop.typeShop == createdShop.shopType)
                {
                    hasMatchingShop = true;

                    Debug.Log($"{name} : shop correspondant trouvé -> {createdShop.shopType}");

                    GoBrowsing(createdShop);

                    break;
                }
            }

            if (hasMatchingShop)
                break;
        }

        if (!hasMatchingShop)
        {
            //Debug.Log($"{name} : aucun shop ne correspond à ses besoins");
        }
    }

    private void OnStoreClosed()
    {
        Debug.Log("On rentre a la maison c'est fini !");
    }

    private void GoBrowsing(ShopCreated shop)
    {
        currentState = CustomerState.browsing;
        points.Clear();

        if (shop.zoneShop == null)
        {
            Debug.LogWarning($"{shop.shopName} n'a pas de zone");
            return;
        }

        ShopZone zone = shop.zoneShop.GetComponentInParent<ShopZone>();
        if (zone == null || zone.meshCollider == null)
        {
            Debug.LogWarning($"{shop.shopName} : ShopZone invalide");
            return;
        }

        Vector3 randomInside = zone.GetRandomPointInside();

        NavPoint browsePoint = new NavPoint
        {
            position = randomInside,
            waitTime = UnityEngine.Random.Range(1.5f, 3f)
        };

        points.Add(browsePoint);
        currentWaitTime = browsePoint.waitTime;

        agent.ResetPath();
        agent.isStopped = false;
        agent.SetDestination(randomInside);
    }
}



[Serializable]
public class NavPoint
{
    public Transform point;
    public Vector3 position;
    public float waitTime;

    public Vector3 GetPosition()
    {
        if (point != null)
            return point.position;

        return position;
    }
}

[Serializable]
public class ShopList
{
    public TypeShop typeShop;
    public List<StockInfoSO> listStockType;
}

public enum NeedType
{
    Food,
    Pee,
    Comfort,
    Energy,
    Distraction
}

[Serializable]
public class Need
{
    [SerializeField] private NeedType needType;
    [SerializeField] private string valueName;
    [Range(0f, 100f)]public float sliderValue;
    [SerializeField] private float minSliderValue;
    [SerializeField] private float maxSliderValue;

    [Header("Random speed ranges")]
    [SerializeField] private float minDecaySpeed = 0.5f;
    [SerializeField] private float maxDecaySpeed = 2f;

    [Header("Random Strength ranges")]
    [SerializeField] private float minDecayStrength = 1f;
    [SerializeField] private float maxDecayStrength  = 5f;

    [Header("Limit slider value")]
    [SerializeField] private float limitSliderValue = 20;

    

    private float decaySpeed;
    private float decayStrength;

    private float currentTimer;
    private bool verifyLimit;

    private CustomerController owner;


    public void Init(CustomerController customer)
    {
        owner = customer;

        sliderValue = UnityEngine.Random.Range(minSliderValue,maxSliderValue);

        decaySpeed = UnityEngine.Random.Range(minDecaySpeed, maxDecaySpeed);
        decayStrength = UnityEngine.Random.Range(minDecayStrength, maxDecayStrength);

        currentTimer = decaySpeed; 
    }

    public void Decrease(GameObject obj)
    {
        currentTimer -= Time.deltaTime;

        if (currentTimer <= 0f)
        {
            sliderValue -= decayStrength;
            sliderValue = Mathf.Clamp(sliderValue, 0, 100);

            currentTimer = decaySpeed; 
        }

        if(sliderValue <= limitSliderValue && !verifyLimit)
        {
            verifyLimit = true;
            //TODO add need in list and do action to do list every time take random range between 2 resource to make diff 
            //Debug.Log("Value " + valueName + "need action for increase value " + obj.name);
            if (!owner.needList.Contains(needType))
            {
                owner.needList.Add(needType);
                owner.currentState = CustomerController.CustomerState.need;
            }
        }
    }

    public void Increase(float value)
    {
        sliderValue += value;
        sliderValue = Mathf.Clamp(sliderValue, 0, 100);

        if (sliderValue <= limitSliderValue)
        {
            if (owner.needList.Contains(needType))
            {
                owner.needList.Remove(needType);
                owner.needList.Add(needType);
            }
        }
        else
        {
            verifyLimit = false;

            if (owner.needList.Contains(needType))
            {
                owner.needList.Remove(needType);
            }
        }
    }

    public float Value => sliderValue;
}
[Serializable]
public class Satisfy
{
    [Range(-100, 100f)]public float sliderValue;

    [SerializeField] private float minSliderValue;
    [SerializeField] private float maxSliderValue;

    [Header("Random Decay Strength ranges")]
    [SerializeField] private float minDecayStrength = 1f;
    [SerializeField] private float maxDecayStrength  = 5f;

    [Header("Random Increase Strength ranges")]
    [SerializeField] private float minIncreaseStrength = 1f;
    [SerializeField] private float maxIncreaseStrength  = 5f;

    [Header("Limit slider value ranges")]
    [SerializeField] private float limitSliderValueMin = -30;
    [SerializeField] private float limitSliderValueMax = -100;

    private float decayStrength;
    private float increaseStregth;
    private float limitSliderValue;
    private CustomerController owner;

    public void Init(CustomerController customer)
    {
        owner = customer;

        sliderValue = UnityEngine.Random.Range(minSliderValue,maxSliderValue);

        decayStrength = UnityEngine.Random.Range(minDecayStrength,maxDecayStrength);
        increaseStregth = UnityEngine.Random.Range(minIncreaseStrength,maxIncreaseStrength);

        limitSliderValue = UnityEngine.Random.Range(limitSliderValueMin,limitSliderValueMax);
    }

    public void Decrease(int multiplyValue)
    {
        sliderValue -= decayStrength * multiplyValue;

        sliderValue = Mathf.Clamp(sliderValue, -100, 100);

        if(sliderValue <= limitSliderValue)
        {
            Debug.Log("BACK HOME IM NOT HAPPY !" + owner.name);
            owner.StartLeaving();
        }
    }

    public void Increase(int multiplyValue)
    {
        sliderValue += increaseStregth * multiplyValue;

        sliderValue = Mathf.Clamp(sliderValue, -100, 100);
    }
    public float Value => sliderValue;
}
