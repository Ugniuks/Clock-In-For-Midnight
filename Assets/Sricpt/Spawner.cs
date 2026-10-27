using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;
public class Spawner : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private Customer customerPrefab;
    [SerializeField] private float minSpawnTime = 5f;
    [SerializeField] private float maxSpawnTime = 20f;


    private float nextSpawnTime;
    private IObjectPool<Customer> customerPool;

    private void Awake()
    {
        // Initialize the object pool for the enemy
        customerPool = new ObjectPool<Customer>(CreateCustomer);
    }
    private void Start()
    {
        ScheduleNextSpawn();
    }


    private Customer CreateCustomer()
    {
        // Instantiate a new enemy from the prefab
        Customer customer = Instantiate(customerPrefab);
        customer.SetPool(customerPool);
        return customer;
    }

    
    // Update is called once per frame
    private void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnCustomer();
            ScheduleNextSpawn();
        }
    }
    private void SpawnCustomer()
    {
        Customer customer = customerPool.Get();

        Transform sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
        customer.GetComponent<NavMeshAgent>().Warp(sp.position);

        customer.OnSpawn(); 
    }

    private void ScheduleNextSpawn()
    {
        nextSpawnTime = Time.time + Random.Range(minSpawnTime, maxSpawnTime);
    }
}

