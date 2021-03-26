using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnvironmentManager : MonoBehaviour
{

    public GameObject shipPrefab;
    public List<Transform> shipSpawns;
    private List<GameObject> activeShips = new List<GameObject>();

    public GameObject fishPrefab;
    public List<Transform> fishSpawns;
    private List<GameObject> activeFishes = new List<GameObject>();


    // UI sliders
    public Slider nFishesSlider;
    public Slider nShipsSlider;

    public Slider fishVisionRangeSlider;
    public Slider shipVisionRangeSlider;
    public Slider harpoonCooldownSlider;
    public Slider harpoonRangeSlider;

    public Toggle competitive;

    void Start() {
        UpdateValues();
        Reset();
    }

    void Update() {
        UpdateValues();
    }

    void UpdateValues() {
        FishSensors.Size = fishVisionRangeSlider.value;
        ShipSensors.Size = shipVisionRangeSlider.value;
        HarpoonLauncher.Cooldown = harpoonCooldownSlider.value;
        Harpoon.Range = harpoonRangeSlider.value;
    }

    public void Reset() {
        ResetOne((int)nShipsSlider.value, shipPrefab, shipSpawns, activeShips);
        ResetOne((int)nFishesSlider.value, fishPrefab, fishSpawns, activeFishes, true);
    }

    void ResetOne(int nObjects, GameObject prefab, List<Transform> spawns, List<GameObject> active, bool randomRot = false) {
        while (active.Count != 0) {
            GameObject.Destroy(active[0]);
            active.RemoveAt(0);
        }

        var newSpawns = new List<Transform>(spawns);
        Shuffle(newSpawns);

        for (int i = 0; i < nObjects; i++) {
            var instance = GameObject.Instantiate(prefab, newSpawns[i].position, newSpawns[i].rotation); 
            instance.name += $" {i}";
            if (randomRot) {
                instance.GetComponent<FishMovement>().Rotation = Random.Range(0f, 360f);
            }
            active.Add(instance);
        }
    }

    public static void Shuffle<T>(IList<T> list) {  
        int n = list.Count;  
        while (n > 1) {  
            n--;  
            int k = Random.Range(0, n + 1);  
            T value = list[k];  
            list[k] = list[n];  
            list[n] = value;  
        }  
    }
}
