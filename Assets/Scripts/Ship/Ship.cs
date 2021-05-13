using UnityEngine;

public class Ship : MonoBehaviour, IShip
{
    public ShipMovement movement;
    public ShipSensors shipSensors;
    public HarpoonLauncher harpoon;
    private ProximitySensor proximitySensor;
    private WallSensors wallSensors;

    // Start is called before the first frame update
    void Start()
    {
        movement.Throttle = 1f;
        proximitySensor = shipSensors.visionSensor;
        wallSensors = shipSensors.wallSensors;
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

        var closestFishes = GetClosesFishes();
        if (closestFishes != null)
        {
            FollowFish(closestFishes.GetValueOrDefault());
            return;
        }

        movement.Helm = 0f;
        movement.Throttle = 1f;
    }

    Vector2? GetClosesFishes()
    {
        Vector2? closest = null;
        float minDistance = float.MaxValue;
        foreach (var fish in proximitySensor.SeenFishes.Values)
        {
            var distance = Vector2.Distance(fish, transform.position);
            if (distance < minDistance)
            {
                closest = fish;
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
        var angle = Vector2.SignedAngle(transform.up, hitVector);
        movement.Throttle = 0.6f;

        movement.Helm = Mathf.Sign(angle) * -0.9f;
    }

    void FollowFish(Vector2 hit)
    {
        Vector2 position = new Vector2(transform.position.x, transform.position.y);

        var hitVector = hit - position;
        var angle = Vector2.SignedAngle(transform.up, hitVector);

        if (Mathf.Abs(angle) < 15)
        {
            movement.Helm = 0f;
        }
        else
        {
            movement.Helm = Mathf.Sign(angle) * 0.5f;
        }

        if (Vector2.Distance(hit, transform.position) < Harpoon.Range)
        {
            harpoon.LookAt(hit);
            harpoon.Fire();
        }
    }

    public void OnKilled(string _)
    {
        // ignored
    }
}
