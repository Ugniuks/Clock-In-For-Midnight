using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    [Header("Hotdog")]
    public float hotdogChance = 1f;   // 100% of customers want a hotdog
    bool wantsHotdog;

    [Header("Dialogue")]
    [SerializeField] private TMP_Text bubble;
    [SerializeField] private AudioSource voice;
    [SerializeField] private AudioClip speakSound;

    void Say(string line)
    {
        Debug.Log($"Say -> bubble: {bubble}, voice: {voice}, clip: {speakSound}");
        if (bubble == null) return;

        bubble.text = line;
        bubble.gameObject.SetActive(true);

        if (voice != null && speakSound != null)
        {
            voice.pitch = Random.Range(0.9f, 1.2f);   // so every customer sounds a bit different
            voice.PlayOneShot(speakSound);
        }
    }
    void HideBubble()
    {
        if (bubble != null) bubble.gameObject.SetActive(false);
    }

    IEnumerator HideBubbleAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        HideBubble();
    }

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
        HideBubble();

        // Only look the points up once, not on every reuse from the pool
        if (allPoints == null)
        {
            var found = GameObject.FindGameObjectsWithTag("WayPoint");
            allPoints = new Transform[found.Length];
            for (int i = 0; i < found.Length; i++)
                allPoints[i] = found[i].transform;

            finalPoint = GameObject.FindGameObjectWithTag("FinalPoint").transform;
        }

        wantsHotdog = Random.value < hotdogChance;
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
        // nothing to buy: skip the line and leave
        if (!wantsHotdog && basket.Count == 0)
        {
            yield return LeaveRoutine();
            yield break;
        }

        Counter counter = finalPoint.GetComponent<Counter>();
        counter.Join(this);

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

        string line = wantsHotdog
        ? (basket.Count > 0 ? "These, and one hotdog, please!" : "One hotdog, please!")
        : "Hello!";
        Say(line);

        yield return counter.Serve(basket, wantsHotdog);
        basket.Clear();
        counter.Leave(this);

        Say("Thank you!");
        StartCoroutine(HideBubbleAfter(2f));

        yield return LeaveRoutine();

        
    }

    IEnumerator LeaveRoutine()
    {
        index = route.Count;                  // route is finished, Update() skips it

        agent.SetDestination(spawnPosition);
        yield return null;                    // give the path a frame to calculate
        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
            yield return null;

        Customerpool.Release(this);
    }
}
