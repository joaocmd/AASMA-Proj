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
    public int tickRate = 500;
    public GameObject marker;
    public Dictionary<string, GameObject> markers = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> _seen = new Dictionary<string, GameObject>();

    private DateTime lastTick;

    void Awake()
    {
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), transform.parent.GetComponent<Collider2D>());
        lastTick = DateTime.Now;
    }

    void OnTriggerExit2D(Collider2D col)
    {
        var name = col.gameObject.name;
        _seen.Remove(name);
        switch (col.gameObject.tag)
        {
            case "Fish":
                SeenShips.Remove(name);
                break;
            case "Harpoon":
                SeenHarpoons.Remove(name);
                break;
            case "Ship":
                SeenShips.Remove(name);
                break;
            default:
                break;
        }

        GameObject.Destroy(markers[name]);
        markers.Remove(name);
    }

    void Update()
    {
        if ((DateTime.Now - lastTick).TotalMilliseconds >= tickRate)
        {
            foreach (var i in _seen)
            {
                var name = i.Key;
                var obj = i.Value;
                var marker = markers[name];
                var pos = new Vector3(
                    obj.transform.position.x + UnityEngine.Random.Range(-noise, noise),
                    obj.transform.position.y + UnityEngine.Random.Range(-noise, noise),
                    obj.transform.position.z);

                markers[name].transform.position = pos;

                switch (obj.tag)
                {
                    case "Fish":
                        SeenShips[name] = pos;
                        break;
                    case "Harpoon":
                        SeenHarpoons[name] = pos;
                        break;
                    case "Ship":
                        SeenShips[name] = pos;
                        break;
                    default:
                        break;
                }

                Debug.Log($"{name}-{pos}");
            }

            lastTick = DateTime.Now;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        var obj = col.gameObject;
        var name = obj.name;

        _seen[name] = obj;
        markers[name] = Instantiate(marker, obj.transform.position, Quaternion.identity);
    }

    private Vector3 positionWithNoise(GameObject obj)
    {
        return new Vector3(
            obj.transform.position.x + UnityEngine.Random.Range(-noise, noise),
            obj.transform.position.y + UnityEngine.Random.Range(-noise, noise),
            obj.transform.position.z);
    }
}
