using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSensors : MonoBehaviour
{

    public static float Size = 8f;
    public ProximitySensor visionSensor;
    public Transform visionVisualizer;
    public WallSensors wallSensors;

    void Update()
    {
        visionVisualizer.localScale = new Vector3(Size, Size, 1f);
        visionSensor.Range = Size;
        wallSensors.Range = Size;
    }
}
