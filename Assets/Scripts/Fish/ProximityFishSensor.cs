using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityFishSensor : MonoBehaviour
{

    private HashSet<string> Seen = new HashSet<string>();
    public bool BoatNearby { get; private set; }

    void OnTriggerExit2D(Collider2D col)
    {
        Seen.Remove(col.gameObject.name);
        if (Seen.Count == 0)
        {
            BoatNearby = false;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Seen.Remove(col.gameObject.name);
        BoatNearby = true;
    }
}
