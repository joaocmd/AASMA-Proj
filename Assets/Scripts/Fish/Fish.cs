using UnityEngine;

public class Fish : MonoBehaviour, IFish
{
    public FishMovement movement;
    public FishSensors sensors;
    private WallSensors wallSensors;
    private ProximitySensor vision;

    // Start is called before the first frame update
    void Start()
    {
        wallSensors = sensors.wallSensors;
        vision = sensors.visionSensor;
    }

    // Update is called once per frame
    void Update()
    {
        var closest = GetClosestWall();
        if (closest != null)
        {
            DodgeWall(closest.GetValueOrDefault());
            return;
        }

        var closestShip = GetClosestShip();
        if (closestShip != null)
        {
            DodgeShip(closestShip.GetValueOrDefault());
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

    void DodgeShip(Vector2 hit)
    {
        Vector2 position = new Vector2(transform.position.x, transform.position.y);

        var hitVector = position - hit;
        movement.Direction = hitVector;
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

    void IFish.OnNotifyFish(Vector2 position)
    {
        // ignore
    }
}
