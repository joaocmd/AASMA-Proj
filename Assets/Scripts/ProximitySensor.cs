using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximitySensor : MonoBehaviour
{

    public Dictionary<string, Vector3> Seen = new Dictionary<string, Vector3>();
    public float noise = 0.5f;
    public int tickRate = 500;
    public GameObject marker;
    public Dictionary<string, GameObject> markers = new Dictionary<string, GameObject>();

    private DateTime lastTick;

    void Start() {
        lastTick = DateTime.Now;
    }

    void OnTriggerExit2D(Collider2D col)
    {
        var name = col.gameObject.name;
        Seen.Remove(name);
        GameObject.Destroy(markers[name]);
        markers.Remove(name);
    }

    void OnTriggerStay2D(Collider2D col)
    {
        Debug.Log((DateTime.Now - lastTick).TotalMilliseconds);
        if ((DateTime.Now - lastTick).TotalMilliseconds < tickRate) {
            return;
        }

        var name = col.gameObject.name;
        var pos = new Vector3(
            col.transform.position.x + UnityEngine.Random.Range(-noise, noise),
            col.transform.position.y + UnityEngine.Random.Range(-noise, noise),
            col.transform.position.z);

        Seen[name] = pos;
        if (markers.ContainsKey(name)) {
            markers[name].transform.position = pos;
        } else {
            markers[name] = Instantiate(marker, pos, Quaternion.identity);
        }
        lastTick = DateTime.Now;
    }
}
