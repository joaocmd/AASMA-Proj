using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WallPosition
{
    LEFT,
    MIDDLE,
    RIGHT
}
public class WallSensors : MonoBehaviour
{

    private int[] raysIncr = { -65, -45, -10, 0, 10, 45, 65 };


    public List<RaycastHit2D> Hits = new List<RaycastHit2D>();
    public HashSet<WallPosition> HitPositions = new HashSet<WallPosition>();


    public float Range { get { return effectiveRange * 2; } set { effectiveRange = value / 2; } }
    private float effectiveRange;

    void Update()
    {
        Hits.Clear();
        HitPositions.Clear();
        for (int i = 0; i < raysIncr.Length; i++)
        {
            Vector3 dir = Quaternion.Euler(0, 0, raysIncr[i]) * transform.up;
            Debug.DrawRay(transform.position, dir * effectiveRange, Color.red, 0.1f);
            // 1 is the default layer
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, effectiveRange, 1);

            if (hit.collider != null)
            {
                Hits.Add(hit);
                if (i <= 1)
                {
                    HitPositions.Add(WallPosition.LEFT);
                }
                else if (i <= 4)
                {
                    HitPositions.Add(WallPosition.MIDDLE);
                }
                else
                {
                    HitPositions.Add(WallPosition.RIGHT);
                }
            }

        }
    }
}
