using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSensors : MonoBehaviour
{

    public static float Size = 8f;
    public ProximitySensor visionSensor;

    void Update()
    {
        visionSensor.transform.localScale = new Vector3(Size, Size, 1f);
    }
}
