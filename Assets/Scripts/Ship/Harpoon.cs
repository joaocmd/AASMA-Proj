using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harpoon : MonoBehaviour
{
    public IShip Parent;

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
        if (Vector3.Distance(transform.position, start) >= Range - 1f)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Fish")
        {
            GameObject.FindWithTag("GameManager").GetComponent<EnvironmentManager>().RemoveFish(other.gameObject);
            Parent.OnKilled(other.gameObject.name);
            Destroy(other.gameObject);
        }
        else if (other.gameObject.tag == "Island")
        {
            Destroy(gameObject);
        }

    }
}
