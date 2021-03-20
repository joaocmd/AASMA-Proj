using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishMovement : MonoBehaviour {

    public static float maxSpeed = 1.1f;
    
    public float _speed;
    public float Speed {
        get { return _speed; }
        set { _speed = Mathf.Clamp(value, 0f, 1f); }
    }

    public float _rotation = 0f;
    public float Rotation {
        get { return _rotation; }
        set { _rotation = value; }
    }

    private Rigidbody2D rb;

    void Awake() {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() {
        rb.MoveRotation(Rotation);
        rb.velocity = transform.up * Speed * maxSpeed;
    }
}
