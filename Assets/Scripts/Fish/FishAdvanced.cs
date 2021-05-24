using UnityEngine;
using System.Collections;

public class FishAdvanced : MonoBehaviour, IFish
{
    private EnvironmentManager environment;
    public FishMovement movement;
    public FishSensors sensors;
    private WallSensors wallSensors;
    private ProximitySensor vision;
    private HarpoonSensor harpoonSensor;
    private Vector2? closestShip;
    private RaycastHit2D? closestWall;
    private Transform previousHarpoon;

    // Start is called before the first frame update
    void Start()
    {
        closestShip = null;
        previousHarpoon = null;
        wallSensors = sensors.wallSensors;
        vision = sensors.visionSensor;
        harpoonSensor = sensors.harpoonSensor;
        environment = GameObject.FindGameObjectWithTag("GameManager").GetComponent<EnvironmentManager>();
        movement.Speed = 1;
    }

    // Update is called once per frame
    void Update()
    {

        closestShip = GetClosestShip();
        closestWall = GetClosestWall();
        Transform closestHarpoon = GetClosestHarpoon();

        if (closestHarpoon != null)
        {
            DodgeHarpoon(closestHarpoon);
            return;
        }

        if (closestShip != null)
        {
            environment.NotifyFish(gameObject);
            DodgeShip(closestShip.GetValueOrDefault());
            return;
        }

        if (closestWall != null)
        {
            DodgeWall(closestWall.GetValueOrDefault());
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

    Transform GetClosestHarpoon()
    {
        Transform closest = null;
        float minDistance = float.MaxValue;
        foreach (var harpoon in harpoonSensor.SeenHarpoons)
        {
            if (harpoon == null)
            {
                continue;
            }
            var distance = Vector2.Distance(harpoon.position, transform.position);
            if (distance < minDistance)
            {
                closest = harpoon;
                minDistance = distance;
            }
        }
        return closest;
    }

    void DodgeWall(RaycastHit2D hit)
    {
        Vector2 position = new Vector2(transform.position.x, transform.position.y);

        var hitVector = hit.point - position;
        var angle = -Vector2.SignedAngle(transform.up, hitVector);
        var random = Random.value;

        if (wallSensors.HitPositions.Contains(WallPosition.LEFT)
            && wallSensors.HitPositions.Contains(WallPosition.RIGHT)
            && !wallSensors.HitPositions.Contains(WallPosition.MIDDLE))
        {
            // keep going forward
        }
        else if (hitVector.magnitude < 2f)
        {
            if ((random < 0.95 || closestShip != null))
            {
                movement.Rotate(Mathf.Sign(angle) * 2f);
            }
            else if (random >= 0.95 && random < 0.99 && closestShip == null)
            {
                movement.Rotate(Mathf.Sign(angle) * 50f);
            }
            else if (random >= 0.99 && closestShip == null)
            {
                movement.Rotate(Mathf.Sign(angle) * 80f);
            }
        }
    }

    void DodgeShip(Vector2 hit)
    {
        Vector2 position = new Vector2(transform.position.x, transform.position.y);
        var distanceVector = (position - hit).normalized;
        var angle = Vector2.SignedAngle(transform.up, distanceVector);

        if (closestWall == null)
        {
            movement.Direction = distanceVector;
        }
        else if (Mathf.Abs(angle) > 150)
        {
            movement.Rotate(Mathf.Sign(angle) * 180f);
        }
        else
        {
            DodgeWall(closestWall.GetValueOrDefault());
        }

    }

    void DodgeHarpoon(Transform harpoon)
    {
        Vector2 position = new Vector2(transform.position.x, transform.position.y);
        var distanceVector = (position - new Vector2(harpoon.position.x, harpoon.position.y)).normalized;
        var angle = Vector2.SignedAngle(harpoon.right, distanceVector);
        movement.Speed = 1f;

        if (previousHarpoon != harpoon)
        {
            movement.Direction = harpoon.right.normalized;
            movement.Rotate(Mathf.Sign(angle) * 90);
            previousHarpoon = harpoon;
        }
        else
        {
            // continue with the same rotation
        }
    }

    void Explore()
    {
        var random = Random.value;
        if (random < 0.999)
        {
            movement.Speed = 1;
        }
        else if (closestShip == null && random >= 0.99)
        {
            movement.Rotate(Random.Range(-20, 20));
        }
    }

    public void OnNotifyFish(Vector2 position)
    {
        if (Vector2.Distance(transform.position, position) < 12.5f)
        {
            DodgeShip(position);
        }
    }

    public void Kill()
    {
        GameObject.FindWithTag("GameManager").GetComponent<EnvironmentManager>().RemoveFish(gameObject);
        Destroy(gameObject);
    }
}
