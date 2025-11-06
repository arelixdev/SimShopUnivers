using System;
using System.Collections.Generic;
using UnityEngine;

public class CustomerController : MonoBehaviour
{
    [SerializeField] private List<NavPoint> points = new List<NavPoint>();

    [SerializeField] private float moveSpeed;


    private float currentWaitTime;

    private void Start()
    {
        currentWaitTime = points[0].waitTime;
    }

    void Update()
    {
        if (points.Count > 0)
        {
            MoveToPoint();
        }
    }

    public void MoveToPoint()
    {
        Vector3 targetPosition = new Vector3(points[0].point.position.x, transform.position.y, points[0].point.position.z);

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        transform.LookAt(targetPosition);

        if (Vector3.Distance(transform.position, targetPosition) < 0.25f)
        {
            currentWaitTime -= Time.deltaTime;

            if (currentWaitTime <= 0)
            {
                StartNextPoint();
            }
        }
    }

    public void StartNextPoint()
    {
        if (points.Count > 0)
        {
            points.RemoveAt(0);

            if (points.Count > 0)
            {
                currentWaitTime = points[0].waitTime;
            }
        }
    }
}

[Serializable]
public class NavPoint
{
    public Transform point;
    public float waitTime;
}
