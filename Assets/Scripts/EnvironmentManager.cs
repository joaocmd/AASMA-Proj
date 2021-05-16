using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnvironmentManager : MonoBehaviour
{
    public bool randomRotation = true;
    public bool training = false;

    public float elapsedTime = 0f;

    public GameObject shipPrefab;
    public List<Transform> shipSpawns;
    public static List<GameObject> ActiveShips = new List<GameObject>();

    public GameObject fishPrefab;
    public List<Transform> fishSpawns;
    public static List<GameObject> ActiveFishes = new List<GameObject>();


    // UI sliders
    public Slider nFishesSlider;
    public Slider nShipsSlider;

    public Slider fishVisionRangeSlider;
    public Slider shipVisionRangeSlider;
    public Slider harpoonCooldownSlider;
    public Slider harpoonRangeSlider;

    public Text elapsedTimeText;

    public Toggle competitive;

    void Start()
    {
        UpdateValues();
        if (!training)
        {
            Reset();
        }
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        UpdateValues();
    }

    void UpdateValues()
    {
        FishSensors.Size = fishVisionRangeSlider.value;
        ShipSensors.Size = shipVisionRangeSlider.value;
        HarpoonLauncher.Cooldown = harpoonCooldownSlider.value;
        Harpoon.Range = harpoonRangeSlider.value;
        elapsedTimeText.text = $"Elapsed Time: {elapsedTime:0.0} seconds";
    }

    public void Reset()
    {
        foreach (var harpoon in GameObject.FindGameObjectsWithTag("Harpoon"))
        {
            Destroy(harpoon);
        }
        ResetOne((int)nShipsSlider.value, shipPrefab, shipSpawns, ActiveShips);
        ResetOne((int)nFishesSlider.value, fishPrefab, fishSpawns, ActiveFishes, true);
        elapsedTime = 0;
        Time.timeScale = 1;
    }

    void ResetOne(int nObjects, GameObject prefab, List<Transform> spawns, List<GameObject> active, bool isFish = false)
    {
        while (active.Count != 0)
        {
            GameObject.Destroy(active[0]);
            active.RemoveAt(0);
        }

        var newSpawns = new List<Transform>(spawns);
        Shuffle(newSpawns);

        for (int i = 0; i < nObjects; i++)
        {
            var instance = GameObject.Instantiate(prefab, newSpawns[i].position, newSpawns[i].rotation);
            instance.name += $" {i}";
            if (isFish)
            {
                instance.GetComponent<FishMovement>().Direction = newSpawns[i].up;
                if (randomRotation) instance.GetComponent<FishMovement>().Rotate(Random.Range(0f, 360f));
            }
            active.Add(instance);
        }
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

    public void RemoveFish(GameObject fish)
    {
        if (training)
        {
            return;
        }

        ActiveFishes.Remove(fish);
        if (ActiveFishes.Count == 0)
        {
            Time.timeScale = 0;
        }
    }

    public void DestroyShip(GameObject ship)
    {
        if (training)
        {
            return;
        }

        RemoveShip(ship);
        ActiveShips.Remove(ship);
        if (ActiveShips.Count == 0)
        {
            Time.timeScale = 0;
        }
    }

    public void RemoveShip(GameObject ship)
    {
        ActiveShips.ForEach(s => s.GetComponent<IShip>().RemoveShip(ship));
    }

    public void NotifyFish(GameObject fish)
    {
        foreach (GameObject other in ActiveFishes)
        {
            if (Object.ReferenceEquals(fish, other))
            {
                continue;
            }
            other.GetComponent<IFish>().OnNotifyFish(fish.transform.position);
        }
    }

    public void NotifyKill(string key)
    {
        ActiveShips.ForEach(s => s.GetComponent<IShip>().OnNotifyKill(key));
    }

    public void UpdateShip(GameObject gameObject, Intention intention)
    {
        ActiveShips.ForEach(s => s.GetComponent<IShip>().UpdateShip(gameObject, intention));
    }

    public void CoordinateHuntWhale(string key, Vector2 shipPos, Vector2 whalePos)
    {
        ActiveShips.ForEach(s => s.GetComponent<IShip>().CoordinateHuntWhale(key, shipPos, whalePos));
    }

    public void NotifyWhaleSighted(Vector2 whalePos)
    {
        ActiveShips.ForEach(s => s.GetComponent<IShip>().NotifyWhaleSighted(whalePos));
    }
}
