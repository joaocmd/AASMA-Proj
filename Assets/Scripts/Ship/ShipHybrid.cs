using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public enum Desire { follow, explore }

public class ShipHybrid : MonoBehaviour, IShip
{
    private EnvironmentManager environment;

    // actuators
    public ShipMovement movement;
    public HarpoonLauncher harpoon;

    //sensors
    public ShipSensors shipSensors;
    private ProximitySensor proximitySensor;
    private WallSensors wallSensors;

    // state
    private Intention intention = null;
    private Dictionary<GameObject, Intention> otherIntentions = new Dictionary<GameObject, Intention>();

    void Start()
    {
        movement.Throttle = 1f;
        proximitySensor = shipSensors.visionSensor;
        wallSensors = shipSensors.wallSensors;
        environment = GameObject.FindGameObjectWithTag("GameManager").GetComponent<EnvironmentManager>();
        // bootstrap intention and tell other ships I'm here
        TryFindWhale(true);
    }

    void Update()
    {
        // hybrid vertical layer architecture, start with reacting to walls/ships

        if (!(ReactiveDecision() || Deliberate()))
        {
            // never happens but condition needs to be attributed or used
            movement.Helm = 0f;
            movement.Throttle = 1f;
        }
    }

    // Used to dodge walls and shoot whales
    bool ReactiveDecision()
    {
        // Always allow firing
        foreach (var whale in proximitySensor.SeenWhales)
        {
            var distance = Vector2.Distance(whale.Value, transform.position);
            if (distance < Harpoon.Range)
            {
                var angle = Vector2.SignedAngle(transform.up, whale.Value - transform.position);
                if (angle < 3f)
                {
                    harpoon.Fire();
                    break;
                }
            }
        }

        // var closestWhale = GetClosesWhale();
        // if (closestWhale != null)
        // {
        //     var closestWhaleValue = closestWhale.GetValueOrDefault();
        //     if (Vector2.Distance(closestWhaleValue.Item2, transform.position) < Harpoon.Range)
        //     {
        //         harpoon.LookAt(closestWhaleValue.Item2);
        //         harpoon.Fire();
        //     }
        // }

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
        if (intention.Desire == Desire.follow)
        {
            FollowWhale();
        }
        else if (intention.Desire == Desire.explore)
        {
            TryFindWhale(false);
        }
        MoveTowards(intention.To);
        return true;
    }

    void FollowWhale()
    {
        bool foundWhale = false;
        foreach (var whale in proximitySensor.SeenWhales)
        {
            if (whale.Key == intention.Key)
            {
                intention.To = whale.Value;
                foundWhale = true;
                environment.UpdateWhalePosition(whale.Key, transform.position, whale.Value);
                break;
            }
        }

        if (!foundWhale && ReachedGoal())
        {
            // keep moving forward, might find it again
            Vector2 newGoal = transform.position + transform.up.normalized * 30f;
            newGoal = new Vector2(Mathf.Clamp(newGoal.x, -17, 17), Mathf.Clamp(newGoal.y, -17, 17));
            ChangeIntention(new Intention(Desire.explore, null, newGoal, false));
        }
    }

    void TryFindWhale(bool changeGoal)
    {
        var closestWhale = GetClosesWhale();
        if (closestWhale != null)
        {
            var whale = closestWhale.GetValueOrDefault();
            ChangeIntention(new Intention(Desire.follow, whale.Item1, whale.Item2, false));
        }
        else
        {
            if (changeGoal || Random.value < 0.0001)
            {
                // TODO: use map bounds insted of hardcoded 34 - 17
                // Debug.Log("Changing Exploration " + Random.value);
                ChangeIntention(new Intention(Desire.explore, null, new Vector2(Random.value * 34 - 17, Random.value * 34 - 17), false));
            }
            else if (ReachedGoal())
            {
                // Debug.Log("Reached Goal" + Random.value);
                ChangeIntention(new Intention(Desire.explore, null, new Vector2(Random.value * 34 - 17, Random.value * 34 - 17), false));
            }
        }
    }

    (string, Vector2)? GetClosesWhale()
    {
        (string, Vector2)? closest = null;
        float minDistance = float.MaxValue;
        foreach (var whale in proximitySensor.SeenWhales)
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

        if (Mathf.Abs(angle) < 10)
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

    private void ChangeIntention(Intention intention)
    {
        environment.UpdateShip(gameObject, intention);
        this.intention = intention;
    }

    public void OnKilled(string key)
    {
        if (intention != null && intention.Key == key)
        {
            TryFindWhale(true);
        }
    }

    public void OnNotifyKill(string key)
    {
        OnKilled(key);
    }

    public void UpdateShip(GameObject ship, Intention intention)
    {
        otherIntentions[ship] = intention;
    }

    public void RemoveShip(GameObject ship)
    {
        otherIntentions.Remove(ship);
    }

    public void UpdateWhalePosition(string key, Vector2 shipPos, Vector2 whalePos)
    {
        // if that's my intention, update it
        if (intention.Key == key)
        {
            intention.To = whalePos;
            return;
        }

        if (otherIntentions.Values.Where(i => i.Key == key).Count() >= 2)
        {
            // two ships already with that intention, let them be
            return;
        }

        GameObject bestCandidate = null;
        float bestScore = float.MinValue;
        Vector2 shipDistanceVec = shipPos - whalePos;
        foreach (var candidate in otherIntentions.Where(s => s.Value.Desire != Desire.follow))
        {
            var candidatePos = new Vector2(candidate.Key.transform.position.x, candidate.Key.transform.position.y);
            // the closer the better (1/distance), the more on the opposite side (angle ~ 180 the better)
            float score = Vector2.Angle(candidatePos - whalePos, shipDistanceVec) / Vector2.Distance(candidatePos, whalePos);
            if (score > bestScore)
            {
                bestScore = score;
                bestCandidate = candidate.Key;
            }
        }

        // if I am the best candidate, broadcast that to everyone
        if (bestCandidate == gameObject)
        {
            ChangeIntention(new Intention(Desire.follow, key, whalePos, true));
        }
    }
}

public class Intention
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