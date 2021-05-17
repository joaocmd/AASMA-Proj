using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WallPosition
{
    LEFT = 0,
    MIDDLE = 1,
    RIGHT = 2
}

public enum ObjectClass
{
    NOTHING,
    WALL,
    SHIP,
}
public class WallSensors : MonoBehaviour
{

    private int[] raysIncr = { -65, -45, -10, 0, 10, 45, 65 };
    public LayerMask mask = 1;


    public List<RaycastHit2D> Hits = new List<RaycastHit2D>();
    public HashSet<WallPosition> HitPositions = new HashSet<WallPosition>();
    public ObjectClass[] HitObjects = { ObjectClass.NOTHING, ObjectClass.NOTHING, ObjectClass.NOTHING };


    public float Range { get { return EffectiveRange * 2; } set { EffectiveRange = value / 2; } }
    public float EffectiveRange { get; private set; }

    void Update()
    {
        Hits.Clear();
        HitPositions.Clear();
        HitObjects = new ObjectClass[] { ObjectClass.NOTHING, ObjectClass.NOTHING, ObjectClass.NOTHING };
        for (int i = 0; i < raysIncr.Length; i++)
        {
            Vector3 dir = Quaternion.Euler(0, 0, raysIncr[i]) * transform.up;
            Debug.DrawRay(transform.position, dir * EffectiveRange, Color.red, 0.02f);
            // 1 is the default layer
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, EffectiveRange, mask);

            if (hit.collider != null)
            {
                Hits.Add(hit);
                if (i <= 1)
                {
                    HitPositions.Add(WallPosition.RIGHT);
                    HitObjects[2] = (hit.collider.tag == "Ship") ? ObjectClass.SHIP : ObjectClass.WALL;
                }
                else if (i <= 4)
                {
                    HitPositions.Add(WallPosition.MIDDLE);
                    HitObjects[1] = (hit.collider.tag == "Ship") ? ObjectClass.SHIP : ObjectClass.WALL;
                }
                else
                {
                    HitPositions.Add(WallPosition.LEFT);
                    HitObjects[0] = (hit.collider.tag == "Ship") ? ObjectClass.SHIP : ObjectClass.WALL;
                }
            }

        }
    }
}
