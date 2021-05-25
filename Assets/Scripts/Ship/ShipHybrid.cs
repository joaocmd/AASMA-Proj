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
        movement.Throttle = 1f;
        if (ReactiveDecision()) return;
        if (Deliberate()) return;
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
                var normalizedDistance = Mathf.Min(5.5f, distance) / 5.5f;
                var angleThreshold = 5f / normalizedDistance;
                var angle = Vector2.Angle(transform.up, whale.Value - transform.position);
                if (angle < angleThreshold)
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

        if (HasShipNearby())
        {
            DodgeShip();
            return true;
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
        var myPosition = new Vector2(transform.position.x, transform.position.y);
        // float previousAngle = Vector2.Angle(transform.up, intention.To - myPosition);
        float previousDistance = Vector2.Distance(intention.To, myPosition);
        foreach (var whale in proximitySensor.SeenWhales)
        {
            if (!foundWhale && whale.Key == intention.Key)
            {
                intention.To = whale.Value;
                environment.CoordinateHuntWhale(whale.Key, transform.position, whale.Value);
                foundWhale = true;
            }
            else if (!foundWhale &&
                Vector2.Angle(transform.up, whale.Value - transform.position) < 40 &&
                Vector2.Distance(transform.position, whale.Value) < previousDistance)
            {
                // allow changing intention to hunt a different whale if it is good enough
                ChangeIntention(new Intention(Desire.follow, whale.Key, transform.position, false));
                environment.CoordinateHuntWhale(whale.Key, transform.position, whale.Value);
                foundWhale = true;
            }
            else
            {
                environment.NotifyWhaleSighted(whale.Value);
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

    bool HasShipNearby()
    {
        return wallSensors.HitObjects.Contains(ObjectClass.SHIP);
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
        if (wallSensors.HitPositions.Count >= 2
            || wallSensors.HitPositions.Contains(WallPosition.MIDDLE))
        {
            movement.Throttle = 0.6f;
        }


        movement.Helm = Mathf.Sign(angle) * -0.9f;
    }

    void DodgeShip()
    {
        // Ship between a wall on the right and a ship on the left
        if (wallSensors.HitObjects[(int)WallPosition.LEFT] == ObjectClass.SHIP &&
            wallSensors.HitObjects[(int)WallPosition.RIGHT] == ObjectClass.WALL)
        {
            movement.Throttle = 0.2f;
            movement.Helm = 0;
        }
        // Ship between a wall on the left and a ship on the right
        else if (wallSensors.HitObjects[(int)WallPosition.RIGHT] == ObjectClass.SHIP &&
            wallSensors.HitObjects[(int)WallPosition.LEFT] == ObjectClass.WALL)
        {
            movement.Throttle = 0.2f;
            movement.Helm = 0;
        }
        // Two ships in front of each other
        else if (wallSensors.HitObjects[(int)WallPosition.MIDDLE] == ObjectClass.SHIP &&
            wallSensors.HitObjects[(int)WallPosition.LEFT] != ObjectClass.SHIP &&
            wallSensors.HitObjects[(int)WallPosition.RIGHT] != ObjectClass.SHIP)
        {
            movement.Throttle = 0.4f;
            movement.Helm -= 0.2f;
        }
        // A ship on the left
        else if (wallSensors.HitObjects[(int)WallPosition.LEFT] == ObjectClass.SHIP)
        {
            movement.Throttle = 0.4f;
            movement.Helm -= 0.2f;
        }
        // A ship on the right
        else if (wallSensors.HitObjects[(int)WallPosition.RIGHT] == ObjectClass.SHIP)
        {
            movement.Helm += 0.2f;
        }
    }

    void MoveTowards(Vector2 goal)
    {
        Vector2 myPosition = new Vector2(transform.position.x, transform.position.y);

        var distanceVector = goal - myPosition;
        var angle = Vector2.SignedAngle(transform.up, distanceVector);

        if (intention.Desire == Desire.follow && distanceVector.magnitude < 3 && angle < 90)
        {
            // slow down when we're closer
            movement.Throttle = 1 - 1 / Mathf.Exp(distanceVector.magnitude - 0.25f);
            Debug.Log(movement.Throttle);
        }

        if (Mathf.Abs(angle) < 3f)
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
        environment.NotifyKill(key);
        OnNotifyKill(key);
    }

    public void OnNotifyKill(string key)
    {
        if (intention != null && intention.Key == key)
        {
            TryFindWhale(true);
        }
    }

    public void UpdateShip(GameObject ship, Intention intention)
    {
        otherIntentions[ship] = intention;
    }

    public void RemoveShip(GameObject ship)
    {
        otherIntentions.Remove(ship);
    }

    public void RemoveAllShips()
    {
        otherIntentions.Clear();
    }

    public void CoordinateHuntWhale(string key, Vector2 shipPos, Vector2 whalePos)
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

    public void NotifyWhaleSighted(Vector2 whalePos)
    {
        if (intention.Desire == Desire.explore && Random.value < 0.001f)
        {
            ChangeIntention(new Intention(Desire.explore, null, whalePos, false));
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