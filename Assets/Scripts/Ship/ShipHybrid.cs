using UnityEngine;

public enum Desire { follow, explore }

public class ShipHybrid : MonoBehaviour, IShip
{
    // actuators
    public ShipMovement movement;
    public HarpoonLauncher harpoon;

    //sensors
    public ShipSensors shipSensors;
    private ProximitySensor proximitySensor;
    private WallSensors wallSensors;

    // state
    private Intention intention = null;

    void Start()
    {
        movement.Throttle = 1f;
        proximitySensor = shipSensors.visionSensor;
        wallSensors = shipSensors.wallSensors;
    }

    void Update()
    {
        // hybrid vertical layer architecture, start with reacting to walls/ships
        if (!(ReactiveDecision() || Deliberate()))
        {
            movement.Helm = 0f;
            movement.Throttle = 1f;
        }

    }

    // Used to dodge walls
    bool ReactiveDecision()
    {
        // Always allow firing
        var closestWhale = GetClosesWhale();
        if (closestWhale != null)
        {
            var closestWhaleValue = closestWhale.GetValueOrDefault();
            if (Vector2.Distance(closestWhaleValue.Item2, transform.position) < Harpoon.Range)
            {
                harpoon.LookAt(closestWhaleValue.Item2);
                harpoon.Fire();
            }
        }

        var closest = GetClosestWall();
        if (closest != null)
        {
            DodgeWall(closest.GetValueOrDefault());
            return true;
        }
        // TODO: dodge other ships
        return false;
    }

    // Used to decide which whale to follow
    bool Deliberate()
    {
        if (intention == null)
        // if no current intention, choose closet whale as a target
        {
            TryFindWhale(true);
        }
        else
        {
            if (intention.Desire == Desire.follow)
            {
                FollowWhale();
            }
            else if (intention.Desire == Desire.explore)
            {
                TryFindWhale(false);
            }
            MoveTowards(intention.To);
        }
        return true;
    }

    void FollowWhale()
    {
        bool foundWhale = false;
        foreach (var whale in proximitySensor.SeenFishes)
        {
            if (whale.Key == intention.Key)
            {
                intention.To = whale.Value;
                foundWhale = true;
                break;
            }
        }

        if (!foundWhale && ReachedGoal())
        {
            // keep moving forward, might find it again
            Debug.Log($"Reached {intention.To}");
            intention = new Intention(Desire.explore, null, transform.position + transform.up.normalized * 5f, false);
            Debug.Log($"Going to {intention.To}");
        }
    }

    void TryFindWhale(bool changeGoal)
    {
        var closestWhale = GetClosesWhale();
        if (closestWhale != null)
        {
            var whale = closestWhale.GetValueOrDefault();
            intention = new Intention(Desire.follow, whale.Item1, whale.Item2, false);
        }
        else
        {
            if (changeGoal || Random.value < 0.0001)
            {
                // TODO: use map bounds insted of hardcoded 34 - 17
                Debug.Log("Changing Exploration " + Random.value);
                intention = new Intention(Desire.explore, null, new Vector2(Random.value * 34 - 17, Random.value * 34 - 17), false);
            }
            else if (ReachedGoal())
            {
                Debug.Log("Reached Goal" + Random.value);
                intention = new Intention(Desire.explore, null, new Vector2(Random.value * 34 - 17, Random.value * 34 - 17), false);
            }
        }
    }

    (string, Vector2)? GetClosesWhale()
    {
        (string, Vector2)? closest = null;
        float minDistance = float.MaxValue;
        foreach (var whale in proximitySensor.SeenFishes)
        {
            var distance = Vector2.Distance(whale.Value, transform.position);
            if (distance < minDistance)
            {
                closest = (whale.Key, whale.Value);
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

    void MoveTowards(Vector2 goal)
    {
        Vector2 myPosition = new Vector2(transform.position.x, transform.position.y);

        var distanceVector = goal - myPosition;
        var angle = Vector2.SignedAngle(transform.up, distanceVector);

        if (Mathf.Abs(angle) < 15)
        {
            movement.Helm = 0f;
        }
        else
        {
            movement.Helm = Mathf.Sign(angle) * 0.5f;
        }
    }

    private bool ReachedGoal()
    {
        Vector2 myPosition = new Vector2(transform.position.x, transform.position.y);
        return Vector2.Distance(myPosition, intention.To) < 2.5f;
    }

    public void OnKilled(string key)
    {
        if (intention != null && intention.Key == key)
        {
            TryFindWhale(true);
        }
    }
}

class Intention
{
    public Desire Desire;
    public string Key;
    public Vector2 To;
    public bool Cooperation;

    public Intention(Desire desire, string key, Vector2 to, bool cooperation)
    {
        this.Desire = desire;
        this.Key = key;
        this.To = to;
        this.Cooperation = cooperation;
    }
}