using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximitySensor : MonoBehaviour
{
    // public property for the outside
    public Dictionary<string, Vector3> Seen = new Dictionary<string, Vector3>();

    public float noise = 0.5f;
    public int tickRate = 500;
    public GameObject marker;
    public Dictionary<string, GameObject> markers = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> _seen = new Dictionary<string, GameObject>();

    private DateTime lastTick;

    void Start() {
        lastTick = DateTime.Now;
    }

    void OnTriggerExit2D(Collider2D col)
    {
        var name = col.gameObject.name;
        _seen.Remove(name);
        Seen.Remove(name);
        GameObject.Destroy(markers[name]);
        markers.Remove(name);
    }

    void Update() {
        if ((DateTime.Now - lastTick).TotalMilliseconds >= tickRate) {
            foreach (var i in _seen) {
                var name = i.Key;
                var obj = i.Value;
                var marker = markers[name];

                var pos = new Vector3(
                    obj.transform.position.x + UnityEngine.Random.Range(-noise, noise),
                    obj.transform.position.y + UnityEngine.Random.Range(-noise, noise),
                    obj.transform.position.z);

                Debug.Log($"{name}-{pos}");
                Seen[name] = pos;
                markers[name].transform.position = pos;
            }

            lastTick = DateTime.Now;
        }
    }

    void OnTriggerEnter2D(Collider2D col) {
        var obj = col.gameObject;
        var name = obj.name;

        _seen[name] = obj;

        var pos = new Vector3(
            obj.transform.position.x + UnityEngine.Random.Range(-noise, noise),
            obj.transform.position.y + UnityEngine.Random.Range(-noise, noise),
            obj.transform.position.z);

        markers[name] = Instantiate(marker, pos, Quaternion.identity);
    }
}
