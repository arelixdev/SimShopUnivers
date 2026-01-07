using System.Collections.Generic;
using UnityEngine;

public class CustomersManager : MonoBehaviour
{
    public static CustomersManager instance; 

    [SerializeField] private List<CustomerController> customerToSpawn = new List<CustomerController>();
    [SerializeField] private float timeBetweenCustomers;

    public List<Transform> allSpawnPoint = new List<Transform>();

    public List<NavPoint> entryPoints = new List<NavPoint>();
    private float spawnCounter;

    private void Awake() {
        instance = this;
    }

    private void Start()
    {
    }

    private void Update()
    {

        spawnCounter -= Time.deltaTime;

        if(spawnCounter <= 0)
        {
            SpawnCustomer();
        }
    }

    public void SpawnCustomer()
    {
        Instantiate(customerToSpawn[Random.Range(0, customerToSpawn.Count)], allSpawnPoint[Random.Range(0, allSpawnPoint.Count)].position, Quaternion.identity);

        spawnCounter = timeBetweenCustomers * Random.Range(0.75f, 1.25f);
    }

    /*public List<NavPoint> GetEntryPoints()
    {
        List<NavPoint> points = new List<NavPoint>();

        points.AddRange(entryPoints);

        return points;
    } */

    /*public List<NavPoint> GetExitPoints()
    {
        List<NavPoint> points = new List<NavPoint>();

        List<NavPoint> temp = new List<NavPoint>();

        temp.AddRange(entryPoints);

        for (int i = temp.Count - 1; i >= 0 ; i--)
        {
            points.Add(temp[i]);
        }

        return points;
    } */
}
