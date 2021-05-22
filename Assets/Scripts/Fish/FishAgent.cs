using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class FishAgent : Agent, IFish
{
    private EnvironmentManager environment;
    public FishMovement movement;
    public FishSensors sensors;
    private WallSensors wallSensors;
    private ProximitySensor vision;
    private HarpoonSensor harpoonSensor;

    public GameObject shipPrefab;
    public List<Transform> fishSpawns = new List<Transform>();
    public List<Transform> shipSpawns = new List<Transform>();

    public void Start()
    {
        wallSensors = sensors.wallSensors;
        vision = sensors.visionSensor;
        harpoonSensor = sensors.harpoonSensor;
        EnvironmentManager.ActiveFishes.Add(gameObject);
    }

    public override void OnEpisodeBegin()
    {
        int nrShips = Random.Range(1, 4);
        foreach (var ship in GameObject.FindGameObjectsWithTag("Ship"))
        {
            Destroy(ship);
        }
        EnvironmentManager.ActiveShips.Clear();

        var newSpawns = new List<Transform>(shipSpawns);
        Shuffle(newSpawns);
        for (int i = 0; i < nrShips; i++)
        {
            var instance = GameObject.Instantiate(shipPrefab, newSpawns[i].position, newSpawns[i].rotation);
            instance.name += $" {i}";
            EnvironmentManager.ActiveShips.Add(instance);
        }

        int spawn = Random.Range(0, fishSpawns.Count);
        transform.position = fishSpawns[spawn].position;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // current rotation
        sensor.AddObservation(transform.localEulerAngles.z / 360.0f);

        // wall sensors
        sensor.AddObservation(wallSensors.HitPositions.Contains(WallPosition.LEFT));
        sensor.AddObservation(wallSensors.HitPositions.Contains(WallPosition.MIDDLE));
        sensor.AddObservation(wallSensors.HitPositions.Contains(WallPosition.RIGHT));

        // closest ship
        Vector3? closestShip = GetClosestShip();
        sensor.AddObservation(closestShip == null);
        float distX = 1f, distY = 1f;
        if (closestShip != null)
        {
            distX = ((closestShip.GetValueOrDefault().x - transform.position.x) / 26f) / 2f + 0.5f;
            distY = ((closestShip.GetValueOrDefault().y - transform.position.y) / 26f) / 2f + 0.5f;
        }
        sensor.AddObservation(distX);
        sensor.AddObservation(distY);

        // closest ship
        Vector3? closestHarpoon = GetClosestHarpoon();
        sensor.AddObservation(closestHarpoon == null);
        distX = 1f; distY = 1f;
        if (closestHarpoon != null)
        {
            distX = ((closestHarpoon.GetValueOrDefault().x - transform.position.x) / 26f) / 2f + 0.5f;
            distY = ((closestHarpoon.GetValueOrDefault().y - transform.position.y) / 26f) / 2f + 0.5f;
        }
        sensor.AddObservation(distX);
        sensor.AddObservation(distY);

        AddReward(1f);
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        float throttle = vectorAction[0];
        float rotate = vectorAction[1];

        movement.Speed = vectorAction[0] / 2 + 0.5f;
        movement.Rotate(vectorAction[1] * 5);
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

    Vector2? GetClosestHarpoon()
    {
        Vector2? closest = null;
        float minDistance = float.MaxValue;
        foreach (var harpoon in harpoonSensor.SeenHarpoons)
        {
            if (harpoon == null)
                continue;

            var distance = Vector2.Distance(harpoon.position, transform.position);
            if (distance < minDistance)
            {
                closest = harpoon.position;
                minDistance = distance;
            }
        }
        return closest;
    }

    public static void Shuffle<T>(IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public void OnNotifyFish(Vector2 position)
    {
        throw new System.NotImplementedException();
    }

    void OnCollisionStay2D(Collision2D other)
    {
        SetReward(-5);
    }

    public void Kill()
    {
        SetReward(-1000);
        EndEpisode();
    }
}
