using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishMovement : MonoBehaviour
{

    public static float maxSpeed = 1.1f;

    public Vector3 Direction
    {
        get { return transform.up; }
        set { transform.up = value; }
    }

    public float _speed;
    public float Speed
    {
        get { return _speed; }
        set { _speed = Mathf.Clamp(value, 0f, 1f); }
    }

    private Rigidbody2D rb;

    void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        rb.velocity = transform.up * Speed * maxSpeed;
    }

    public void Rotate(float angle)
    {
        transform.up = Quaternion.Euler(0, 0, angle) * transform.up;
    }
}
