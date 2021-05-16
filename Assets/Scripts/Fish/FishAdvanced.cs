using UnityEngine;
using System.Collections;

public class FishAdvanced : MonoBehaviour, IFish
{
    private EnvironmentManager environment;
    public FishMovement movement;
    public FishSensors sensors;
    private WallSensors wallSensors;
    private ProximitySensor vision;

    private bool reactToShip = true;

    // Start is called before the first frame update
    void Start()
    {
        wallSensors = sensors.wallSensors;
        vision = sensors.visionSensor;
        environment = GameObject.FindGameObjectWithTag("GameManager").GetComponent<EnvironmentManager>();
    }

    // Update is called once per frame
    void Update()
    {

        var closestShip = GetClosestShip();
        if (closestShip != null && reactToShip)
        {
            environment.NotifyFish(gameObject);
            StartCoroutine(DodgeShip(closestShip.GetValueOrDefault()));
            reactToShip = false;
            return;
        }

        var closest = GetClosestWall();
        if (closest != null)
        {
            DodgeWall(closest.GetValueOrDefault());
            return;
        }

        Explore();
    }


    Vector2? GetClosestShip()
    {
        Vector2? closest = null;
        float minDistance = float.MaxValue;
        foreach (var ship in vision.SeenShips.Values)
        {
            var distance = Vector2.Distance(ship, transform.position);
            if (distance < minDistance)
            {
                closest = ship;
                minDistance = distance;
            }
        }
        return closest;
    }
    RaycastHit2D? GetClosestWall()
    {
        RaycastHit2D? closest = null;
        float minDistance = float.MaxValue;
        foreach (var hit in wallSensors.Hits)
        {
            var distance = Vector2.Distance(hit.point, transform.position);
            if (distance < minDistance)
            {
                closest = hit;
                minDistance = distance;
            }
        }
        return closest;
    }

    void DodgeWall(RaycastHit2D hit)
    {
        Vector2 position = new Vector2(transform.position.x, transform.position.y);

        var hitVector = hit.point - position;
        // Debug.Log($"Hit Vector: {hitVector}");
        var angle = -Vector2.SignedAngle(transform.up, hitVector);
        if (wallSensors.HitPositions.Contains(WallPosition.LEFT)
            && wallSensors.HitPositions.Contains(WallPosition.RIGHT)
            && !wallSensors.HitPositions.Contains(WallPosition.MIDDLE))
        {
            // keep going forward
        }
        else if (Random.value < 0.99)
        {
            movement.Rotate(Mathf.Sign(angle) * 2f);
        }
        else
        {
            movement.Rotate(Mathf.Sign(angle) * 90f);
        }
    }

    IEnumerator DodgeShip(Vector2 hit)
    {
        Vector2 position = new Vector2(transform.position.x, transform.position.y);

        var distanceVector = (position - hit).normalized;
        movement.Direction = distanceVector;

        int incr = 15;
        // try to decide where to dodge, prefer dodging straight out, if not possible then to 15 degrees
        // off straight out, then 30, etc...
        for (int i = 0; incr * i < 170; i++)
        {
            Vector3 left = Quaternion.Euler(0, 0, -(incr * i)) * distanceVector;
            Vector3 right = Quaternion.Euler(0, 0, (incr * i)) * distanceVector;

            // 1 is the default layer
            Debug.DrawRay(transform.position, left * wallSensors.EffectiveRange, Color.magenta, 0.02f);
            RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, left, wallSensors.EffectiveRange, 1);
            if (raycastHit.collider != null)
            {
                movement.Direction = left;
            }

            // 1 is the default layer
            Debug.DrawRay(transform.position, right * wallSensors.EffectiveRange, Color.magenta, 0.02f);
            raycastHit = Physics2D.Raycast(transform.position, right, wallSensors.EffectiveRange, 1);
            if (raycastHit.collider != null)
            {
                movement.Direction = right;
            }
        }

        yield return new WaitForSeconds(2.5f);
        reactToShip = true;
    }

    void Explore()
    {
        if (Random.value < 0.95)
        {
            movement.Speed = 1;
        }
        else
        {
            movement.Rotate(Random.Range(-5, 5));
        }
    }

    public void OnNotifyFish(Vector2 position)
    {
        if (Vector2.Distance(transform.position, position) < 12.5f)
        {
            StartCoroutine(DodgeShip(position));
        }
    }

    public void Kill()
    {
        GameObject.FindWithTag("GameManager").GetComponent<EnvironmentManager>().RemoveFish(gameObject);
        Destroy(gameObject);
    }
}
