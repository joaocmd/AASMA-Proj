using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipSensors : MonoBehaviour
{

    public static float Size = 8f;
    public ProximitySensor visionSensor;
    public WallSensors wallSensors;
    public Transform visionVisualizer;

    void Update()
    {
        visionVisualizer.localScale = new Vector3(Size, Size, 1f);
        visionSensor.Range = Size;
        wallSensors.Range = 7.5f;
    }
}
