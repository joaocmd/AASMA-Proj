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

    // Start is called before the first frame update
    void Start()
    {
        closestShip = null;
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
            Dodgeharpoon(closestHarpoon);
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
        foreach (var hit in harpoonSensor.SeenHarpoons)
        {
            var distance = Vector2.Distance(hit.position, transform.position);
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
        var angle = Vector2.Angle(transform.up, distanceVector);

        if (closestWall == null)
        {
            movement.Direction = distanceVector;
        }
        else if (angle > 150)
        {
            movement.Rotate(Mathf.Sign(angle) * 180f);
        }
        else
        {
            DodgeWall(closestWall.GetValueOrDefault());
        }

    }

    void Dodgeharpoon(Transform harpoon)
    {
        Vector2 position = new Vector2(transform.position.x, transform.position.y);
        var distanceVector = (position - new Vector2(harpoon.position.x, harpoon.position.y)).normalized;
        var angle = Vector2.SignedAngle(transform.up, distanceVector);
        var toRotate = Mathf.Abs(90 - angle);
        var isAboveTheHarpoon = Mathf.Sign(transform.position.y - harpoon.position.y) == 1;

        Debug.Log(angle);
        Debug.Log(isAboveTheHarpoon);

        if (angle < 80 && angle >= 0)
        {
            movement.Rotate(-toRotate + (!isAboveTheHarpoon ? 180f : 0f));
        }
        else if (angle > 110)
        {
            movement.Rotate(toRotate + (!isAboveTheHarpoon ? 180f : 0f));
        }
        else if (angle > -80 && angle < 0)
        {
            movement.Rotate(+toRotate + (isAboveTheHarpoon ? 180f : 0f));
        }
        else if (angle < -110)
        {
            movement.Rotate(-toRotate + (isAboveTheHarpoon ? 180f : 0f));
        }

        movement.Speed = 1f;
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
