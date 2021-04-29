using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSensors : MonoBehaviour
{

    private int[] raysIncr = { -25, -15, 0, 15, 25 };

    public List<RaycastHit2D> Hits = new List<RaycastHit2D>();

    public float Range { get { return effectiveRange * 2; } set { effectiveRange = value / 2; } }
    private float effectiveRange;

    void Update()
    {
        Hits.Clear();
        for (int i = 0; i < raysIncr.Length; i++)
        {
            Vector3 dir = Quaternion.Euler(0, 0, raysIncr[i]) * transform.up;
            Debug.DrawRay(transform.position, dir * effectiveRange, Color.red, 0.1f);
            // 1 is the default layer
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, effectiveRange, 1);

            if (hit.collider != null)
            {
                Hits.Add(hit);
                // Debug.Log($"{hit.collider.transform.name}-{hit.point}");
            }

        }
    }
}
