using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WorkerController : MonoBehaviour
{
    public TypeWorkers typeWork = TypeWorkers.Standby;

    [SerializeField] private List<NavPoint> points = new List<NavPoint>();
    [SerializeField] private Animator animator;
    [SerializeField] private float moveSpeed;

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

    public void DoAction(string shopName, TypeWorkers work)
    {
        switch(work)
        {
            case TypeWorkers.Sellers:
                Debug.Log($"Go to {shopName} and go sell");
                break;
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
