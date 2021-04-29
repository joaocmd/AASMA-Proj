using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class Fish : MonoBehaviour
{

    public FishMovement movement;
    public FishSensors sensors;
    private WallSensors wallSensors;
    private ProximitySensor vision;

    // Start is called before the first frame update
    void Start()
    {
        wallSensors = sensors.wallSensors;
        vision = sensors.visionSensor;
    }

    // Update is called once per frame
    void Update()
    {
        var closest = GetClosestWall();
        if (closest != null)
        {
            DodgeWall(closest.GetValueOrDefault());
            return;
        }

        var closestShip = GetClosestShip();
        if (closestShip != null)
        {
            DodgeShip(closestShip.GetValueOrDefault());
            return;
        }

        MoveForward();
    }


    Vector2? GetClosestShip()
    {
        Vector2? closest = null;
        float minDistance = float.MaxValue;
        foreach (var ship in vision.SeenShips.Values)
        {
            var distance = Vector2.Distance(ship, transform.position);
            if (distance < minDistance)
            {
                closest = ship;
                minDistance = distance;
            }
        }
        return closest;
    }
    RaycastHit2D? GetClosestWall()
    {
        RaycastHit2D? closest = null;
        float minDistance = float.MaxValue;
        foreach (var hit in wallSensors.Hits)
        {
            var distance = Vector2.Distance(hit.point, transform.position);
            if (distance < minDistance)
            {
                closest = hit;
                minDistance = distance;
            }
        }
        return closest;
    }

    void DodgeWall(RaycastHit2D hit)
    {
        Vector2 position = new Vector2(transform.position.x, transform.position.y);

        var hitVector = hit.point - position;
        var angle = Vector2.Angle(transform.up, hitVector);
        if (angle > 0)
        {
            movement.Rotation += 90;
        }
        else
        {
            movement.Rotation -= 90;
        }
    }

    void DodgeShip(Vector2 hit)
    {
        Vector2 position = new Vector2(transform.position.x, transform.position.y);

        var hitVector = hit - position;
        movement.Rotation += 180;
    }

    void MoveForward()
    {
        movement.Speed = 1;
    }
}
