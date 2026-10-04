using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

public class Customer : MonoBehaviour
{
    [Header("Route")]
    public int minStops = 1; // Minimum number of stops before reaching the final point
    public int maxStops = 10;

    NavMeshAgent agent;
    List<Transform> route = new List<Transform>();
    int index;

    private IObjectPool<Customer> Customerpool;
    public void SetPool(IObjectPool<Customer> pool)
    {
        Customerpool = pool;
    }

    Transform[] allPoints;
    Transform finalPoint;
    public void OnSpawn()
    {
        agent = GetComponent<NavMeshAgent>();
        route.Clear();
        index = 0;

        // Only look the points up once, not on every reuse from the pool
        if (allPoints == null)
        {
            var found = GameObject.FindGameObjectsWithTag("WayPoint");
            allPoints = new Transform[found.Length];
            for (int i = 0; i < found.Length; i++)
                allPoints[i] = found[i].transform;

            finalPoint = GameObject.FindGameObjectWithTag("FinalPoint").transform;
        }

        BuildRoute();
        MoveToNext();
    }

    void BuildRoute()
    {
        var pool = new List<Transform>(allPoints);
        int max = Mathf.Min(maxStops, pool.Count);
        int count = Random.Range(Mathf.Min(minStops, max), max + 1);

        for (int i = 0; i < count; i++)
        {
            int pick = Random.Range(0, pool.Count);
            route.Add(pool[pick]);
            pool.RemoveAt(pick);        // no repeats
        }
        route.Add(finalPoint);          // always last
        Customerpool.Release(this);
    }
    void MoveToNext()
    {
        if (index < route.Count)
            agent.SetDestination(route[index].position);
    }


    // Update is called once per frame
    void Update()
    {
        if (index >= route.Count || agent.pathPending)
        {
            return;
        }
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            index++;
            if (index < route.Count)
            {
                agent.SetDestination(route[index].position);
            }
        }
    }
}
