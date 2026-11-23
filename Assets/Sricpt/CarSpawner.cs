using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CarSpawner : MonoBehaviour
{
    public DriveThroughCar[] carPrefabs;
    public Transform[] queueSlots;     
    public Transform finish;

    public float minSpawnTime = 4f;    
    public float maxSpawnTime = 12f;   
    public float minGapFromSpawn = 6f;

    public GasRegister register;

    readonly List<DriveThroughCar> line = new List<DriveThroughCar>();
    float timer;

    public float waitBeforeReaction = 2f;   
    public float waitBeforeLeaving = 2f;
    bool finishing;


    void Start()
    {
        timer = Random.Range(minSpawnTime, maxSpawnTime);
    }

    void Update()
    {
        
        timer -= Time.deltaTime;
        if (timer > 0f || line.Count >= queueSlots.Length) return;

        if (line.Count > 0)
        {
            var last = line[line.Count - 1];
            if (Vector3.Distance(last.transform.position, transform.position) < minGapFromSpawn)
                return;
        }

        SpawnCar();
        timer = Random.Range(minSpawnTime, maxSpawnTime);
    }

    void SpawnCar()
    {
        Debug.Log("Spawning car");

        var prefab = carPrefabs[Random.Range(0, carPrefabs.Length)];
        var car = Instantiate(prefab, transform.position, transform.rotation);
        line.Add(car);
        car.Init(this, queueSlots[line.Count - 1]);
    }

    public void SendCarAway() => StartCoroutine(FinishOrder(false));
    public void SendCarAwayAngry() => StartCoroutine(FinishOrder(true));

    IEnumerator FinishOrder(bool angry)
    {
        if (line.Count == 0 || finishing) yield break;
        finishing = true;

        var front = line[0];
        front.HideBubble();                                   
        yield return new WaitForSeconds(waitBeforeReaction);  

        if (front != null) front.React(angry);                
        yield return new WaitForSeconds(waitBeforeLeaving);

        if (front != null)
        {
            line.Remove(front);
            front.Leave(finish);

            for (int i = 0; i < line.Count; i++)
                line[i].MoveTo(queueSlots[i]);
        }

        finishing = false;
    }


    public void CarArrived(DriveThroughCar car)
    {
        if (line.Count > 0 && line[0] == car)
            car.StartOrder(register);
    }
}
