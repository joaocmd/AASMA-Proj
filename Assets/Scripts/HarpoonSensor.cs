using System;
using System.Collections.Generic;
using UnityEngine;

public class HarpoonSensor : MonoBehaviour
{
    List<Transform> SeenHarpoons = new List<Transform>();
    public int tickRate = 150;
    public float Range;
    private DateTime lastTick;

    // Start is called before the first frame update
    void Start()
    {
        lastTick = DateTime.Now;
    }

    // Update is called once per frame
    void Update()
    {
        var harpoons = EnvironmentManager.ActiveHarpoons;
        if ((DateTime.Now - lastTick).TotalMilliseconds >= tickRate)
        {
            SeenHarpoons.Clear();
            harpoons.ForEach(t =>
            {
                if (t == null)
                    return;

                if (Vector2.Distance(transform.position, t.position) < Range / 2f)
                {
                    SeenHarpoons.Add(t);
                }
            });
            lastTick = DateTime.Now;
        }
    }
}
