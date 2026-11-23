using UnityEngine;

public class DriveThroughCar : MonoBehaviour
{
    public float speed = 4f;
    public float turnSpeed = 4f;
    public float slowDownDistance = 3f;

    CarSpawner spawner;
    Transform target;
    bool leaving, arrived;

    DTCustomer customer;

    void Awake() => customer = GetComponentInChildren<DTCustomer>(true);

    public void StartOrder(GasRegister register)
    {
        if (customer != null) customer.StartOrder(register);
    }

    public void Init(CarSpawner s, Transform firstTarget)
    {
        spawner = s;
        MoveTo(firstTarget);
    }

    public void MoveTo(Transform t)
    {
        target = t;
        arrived = false;
    }

    public void Leave(Transform finish)
    {
        leaving = true;
        target = finish;
        arrived = false;
        Destroy(gameObject, 5f);
    }

    public void HideBubble()
    {
        if (customer != null) customer.HideBubble();
    }

    public void React(bool angry)
    {
        if (customer == null) return;
        if (angry) customer.SayAngry();
        else customer.SayThanks();
    }

    void Update()
    {
        if (target == null || arrived) return;

        Vector3 toTarget = target.position - transform.position;
        float dist = toTarget.magnitude;
        
        if (dist < 0.05f)
        {
            arrived = true;
            if (leaving) { Destroy(gameObject); return; }

            transform.rotation = target.rotation;
            spawner.CarArrived(this);
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
    }
}
