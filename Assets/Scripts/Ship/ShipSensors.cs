using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipSensors : MonoBehaviour {

    public float visionSize = 8f;
    public ProximityShipSensor visionSensor;

    void Start() {
        visionSensor.transform.localScale = new Vector3(visionSize, visionSize, 1f);
    }
}
