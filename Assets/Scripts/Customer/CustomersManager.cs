using System.Collections.Generic;
using UnityEngine;

public class CustomersManager : MonoBehaviour
{
    public static CustomersManager instance; 

    [SerializeField] private List<CustomerController> customerToSpawn = new List<CustomerController>();
    [SerializeField] private float timeBetweenCustomers;

    [SerializeField] private List<NavPoint> entryPoints = new List<NavPoint>();
    private float spawnCounter;

    private void Awake() {
        instance = this;
    }

    private void Start()
    {
        SpawnCustomer();
    }

    private void Update()
    {
        /*spawnCounter -= Time.deltaTime;

        if(spawnCounter <= 0)
        {
            SpawnCustomer();
        }*/
    }

    public void SpawnCustomer()
    {
        Instantiate(customerToSpawn[Random.Range(0, customerToSpawn.Count)]);

        spawnCounter = timeBetweenCustomers * Random.Range(0.75f, 1.25f);
    }

    public List<NavPoint> GetEntryPoints()
    {
        List<NavPoint> points = new List<NavPoint>();

        /*if(Random.value < 0.5)
        {
            
        }*/

        points.AddRange(entryPoints);

        return points;
    } 

    public List<NavPoint> GetExitPoints()
    {
        List<NavPoint> points = new List<NavPoint>();

        List<NavPoint> temp = new List<NavPoint>();

        /*if(Random.value < 0.5)
        {
            
        }*/

        temp.AddRange(entryPoints);

        for (int i = temp.Count - 1; i >= 0 ; i--)
        {
            points.Add(temp[i]);
        }

        return points;
    } 
}
