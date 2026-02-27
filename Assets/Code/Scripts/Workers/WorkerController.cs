using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.PlayerLoop;

public class WorkerController : MonoBehaviour
{
    public TypeWorkers typeWork = TypeWorkers.Standby;

    [SerializeField] private List<NavPoint> points = new List<NavPoint>();
    [SerializeField] private Animator animator;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float checkoutTimerAction;

    private float currentCheckoutTimerAction;

    private Checkout currentCheckout;

    private NavMeshAgent agent;
    private float currentWaitTime;
    private bool hasReachedCurrentPoint = false;

    private void Awake() {
        agent = GetComponent<NavMeshAgent>();
    }

    internal void OpenUIMenu()
    {
        WorkerMenu.instance.OpenMenu(this);
    }

    private void Update()
    {
        if(points.Count > 0)
        {
            MoveToPoint();
        }
    }

    public void DoAction(string shopName, TypeWorkers work)
    {
        switch(work)
        {
            case TypeWorkers.Sellers:
                ShopCreated shop = PanelShopMaster.instance.GetShopByName(shopName);

                if (shop == null)
                    return;

                ShopZone zone = shop.zoneShop.GetComponentInParent<ShopZone>();

                if (zone != null)
                {
                    //Check if checkout 
                    Vector3 targetPosition = Vector3.zero;

                    if(zone.checkoutsInZone.Count > 0)
                    {
                        for (int i = 0; i < zone.checkoutsInZone.Count; i++)
                        {
                            if(zone.checkoutsInZone[i].sellerInCheckout == null)
                            {
                                points.Clear();
                                NavPoint checkoutPoint = new NavPoint
                                {
                                    point = zone.checkoutsInZone[i].GetSellerPoint(),
                                    waitTime = 0.5f
                                };
                                points.Add(checkoutPoint);
                                zone.checkoutsInZone[i].sellerInCheckout = this;
                                currentCheckout = zone.checkoutsInZone[i];
                                currentCheckoutTimerAction = checkoutTimerAction;
                                currentWaitTime = checkoutPoint.waitTime;
                                return;
                            }
                        }
                    }
                    
                }
                break;

        }
    }

    public void MoveToPoint()
    {
        if (points.Count == 0) return;

        Vector3 targetPosition = points[0].point.transform.position;
        targetPosition.y = transform.position.y;

        if (agent.isOnNavMesh)
        {
            agent.SetDestination(targetPosition);
        }
        animator.SetBool("IsMoving", agent.velocity.magnitude > 0.1f);

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.isStopped = true;

            if (currentCheckout != null)
            {
                RotateTowards(currentCheckout.transform.position);
            }

            currentWaitTime -= Time.deltaTime;

            if(currentCheckout != null)
            {
                currentCheckoutTimerAction -= Time.deltaTime;
                if(currentCheckoutTimerAction <= 0)
                {
                    DoCheckoutAction();
                     currentCheckoutTimerAction = checkoutTimerAction;
                } 
            }

            /*if (currentWaitTime <= 0)
            {
                if (currentCheckout != null)
                {
                    DoCheckoutAction();
                }
                StartNextPoint();
            }*/
                
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
            
        }
    }

    private void DoCheckoutAction()
    {
        if(currentCheckout.customersInQueue.Count == 0)
            return;
        
        currentCheckout.WorkerActions();
    }

    private void RotateTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0f;

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }
}

public enum TypeWorkers
{
    Sellers,
    Storage,
    Janitor,
    Officier,
    Cooker,
    Standby
}
