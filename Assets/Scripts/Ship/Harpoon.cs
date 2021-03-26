using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harpoon : MonoBehaviour
{
    public GameObject Parent;

    public static float Speed = 8f;
    public static float Range = 4.5f;

    private Vector3 start;
    public void Start()
    {
        this.start = transform.position;
        GetComponent<Rigidbody2D>().velocity = transform.right * Speed;
    }

    public void Update()
    {
        if (Vector3.Distance(transform.position, start) >= Range)
        {
            Destroy(gameObject);
        }
    }
}
