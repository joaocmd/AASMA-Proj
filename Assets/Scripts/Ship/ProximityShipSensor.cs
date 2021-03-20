using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityShipSensor : MonoBehaviour
{

    public Dictionary<string, Vector3> Seen = new Dictionary<string, Vector3>();

    void OnTriggerExit2D(Collider2D col)
    {
        Seen.Remove(col.gameObject.name);
    }

    void OnTriggerStay2D(Collider2D col)
    {
        Seen[col.gameObject.name] = col.transform.position;
    }
}
