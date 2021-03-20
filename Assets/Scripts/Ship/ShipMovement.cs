using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMovement : MonoBehaviour {

    public static float rotationSpeed = 70f;
    public static float speed = 1.8f;

    // TODO: make this private
    public float _throttle = 0f;
    public float Throttle {
        get { return _throttle; }
        set {
            _throttle = Mathf.Clamp(value, 0f, 1f);
        }
    }

    // TODO: make this private
    public float _helm = 0f;
    public float Helm {
        get { return _helm; }
        set {
            _helm = Mathf.Clamp(value, -1f, 1f);
        }
    }

    private Rigidbody2D rb;

    void Awake() {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() {
        rb.AddForce(transform.up * Throttle * speed);
        var locVel = transform.InverseTransformDirection(rb.velocity);
        var impulse = rotationSpeed * Mathf.Deg2Rad * Helm * locVel.y;
        rb.AddTorque(impulse);
    }
}
