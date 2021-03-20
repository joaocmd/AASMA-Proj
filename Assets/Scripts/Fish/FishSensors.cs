using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSensors : MonoBehaviour
{

    public float visionSize = 8f;
    public ProximitySensor visionSensor;

    void Start()
    {
        visionSensor.transform.localScale = new Vector3(visionSize, visionSize, 1f);
    }
}
