using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class ProximitySensor : MonoBehaviour
{
    // public property for the outside
    public Dictionary<string, Vector3> SeenShips = new Dictionary<string, Vector3>();
    public Dictionary<string, Vector3> SeenHarpoons = new Dictionary<string, Vector3>();
    public Dictionary<string, Vector3> SeenFishes = new Dictionary<string, Vector3>();

    public float noise = 0.5f;
    public int tickRate = 300;
    public GameObject marker;
    public Dictionary<string, GameObject> markers = new Dictionary<string, GameObject>();

    private DateTime lastTick;

    public float Range { get { return effectiveRange * 2 - 0.5f; } set { effectiveRange = value / 2 + 0.5f; } }
    private float effectiveRange;

    void Awake()
    {
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), transform.GetComponent<Collider2D>());
        lastTick = DateTime.Now;
    }

    void Update()
    {
        var ships = EnvironmentManager.ActiveShips;
        if ((DateTime.Now - lastTick).TotalMilliseconds >= tickRate)
        {
            UpdateOne(EnvironmentManager.ActiveShips, SeenShips);
            UpdateOne(EnvironmentManager.ActiveFishes, SeenFishes);
            lastTick = DateTime.Now;
        }
    }

    private void UpdateOne(List<GameObject> target, Dictionary<string, Vector3> storage)
    {
        storage.Clear();
        foreach (GameObject obj in target)
        {
            if (obj == gameObject)
            {
                continue;
            }

            if (Vector2.Distance(transform.position, obj.transform.position) < Range / 2f)
            {
                // Debug.Log($"{obj.transform.name} is in range");
                RaycastHit2D hit = Physics2D.Raycast(transform.position, obj.transform.position - transform.position);
                Debug.DrawLine(transform.position, obj.transform.position, Color.green, 0.1f);
                // Debug.Log($"{hit.transform.name} was hit");
                if (hit.collider.gameObject == obj)
                {
                    // no obstructions and in range
                    var pos = new Vector3(
                        obj.transform.position.x + UnityEngine.Random.Range(-noise, noise),
                        obj.transform.position.y + UnityEngine.Random.Range(-noise, noise),
                        obj.transform.position.z);

                    storage[obj.gameObject.name] = pos;
                    continue;
                }
            }
            SeenShips.Remove(obj.gameObject.name);
        }
    }

    private Vector3 positionWithNoise(GameObject obj)
    {
        return new Vector3(
            obj.transform.position.x + UnityEngine.Random.Range(-noise, noise),
            obj.transform.position.y + UnityEngine.Random.Range(-noise, noise),
            obj.transform.position.z);
    }
}
