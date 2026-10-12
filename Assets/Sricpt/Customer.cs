using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

public class Customer : MonoBehaviour
{
    [Header("Route")]
    public int minStops = 1; // Minimum number of stops before reaching the final point
    public int maxStops = 10;
    Vector3 spawnPosition;

    NavMeshAgent agent;
    List<Transform> route = new List<Transform>();
    int index;

    private IObjectPool<Customer> Customerpool;

    [Header("Shopping")]
    public float pickChance = 0.6f;
    public float browseTime = 1.5f;
    public List<GameObject> basket = new List<GameObject>();
    bool busy;

    public void SetPool(IObjectPool<Customer> pool)
    {
        Customerpool = pool;
    }

    Transform[] allPoints;
    Transform finalPoint;
    public void OnSpawn()
    {
        agent = GetComponent<NavMeshAgent>();
        spawnPosition = transform.position;   // new
        route.Clear();
        index = 0;
        basket.Clear();
        busy = false;

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
    }
    void MoveToNext()
    {
        if (index < route.Count)
            agent.SetDestination(route[index].position);
    }


    // Update is called once per frame
    void Update()
    {
        if (busy || index >= route.Count || agent.pathPending) return;

        if (agent.remainingDistance <= agent.stoppingDistance)
            StartCoroutine(ArriveRoutine());
    }

    IEnumerator ArriveRoutine()
    {
        busy = true;
        Transform point = route[index];

        if (point == finalPoint)
        {
            yield return CheckoutRoutine();
            yield break;
        }

        Shelf shelf = point.GetComponent<Shelf>();
        if (shelf != null)
        {
            yield return new WaitForSeconds(browseTime);
            if (Random.value < pickChance)
                basket.Add(shelf.itemPrefab);
        }

        index++;

        if (route[index] == finalPoint)
        {
            yield return CheckoutRoutine();   // go join the line instead of walking to the register
            yield break;
        }

        agent.SetDestination(route[index].position);
        busy = false;
    }

    IEnumerator CheckoutRoutine()
    {
        Counter counter = finalPoint.GetComponent<Counter>();
        counter.Join(this);

        // walk to my place in line, moving up when the line moves
        int lastSpot = -1;
        while (true)
        {
            int spot = counter.PositionInLine(this);
            if (spot != lastSpot)
            {
                agent.SetDestination(counter.GetSpot(spot));
                lastSpot = spot;
            }

            if (spot == 0 && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                break;

            yield return null;
        }

        yield return counter.Serve(basket);
        basket.Clear();
        counter.Leave(this);          // the next customer moves up now
        index = route.Count;

        // walk back to the spawn point and disappear
        agent.SetDestination(spawnPosition);
        yield return null;
        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
            yield return null;

        Customerpool.Release(this);
    }
}
