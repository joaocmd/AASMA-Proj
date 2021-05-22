using System;
using System.Collections.Generic;
using UnityEngine;

public class HarpoonSensor : MonoBehaviour
{
    public List<Transform> SeenHarpoons = new List<Transform>();
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
        if ((DateTime.Now - lastTick).TotalMilliseconds >= tickRate)
        {
            SeenHarpoons.Clear();
            EnvironmentManager.ActiveHarpoons.ForEach(t =>
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
