using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarpoonLauncher : MonoBehaviour
{

    public static float Cooldown = 3f;
    public SpriteRenderer sprite;

    public GameObject harpoonPrefab;
    public GameObject parent;

    public float Timer { get; private set; } = 0f;

    void Update() {
        Timer -= Time.deltaTime;
        if (Timer <= 0f && !sprite.enabled) {
            sprite.enabled = true;
        }

        if (Input.GetKeyDown(KeyCode.Return)) {
            Fire();
        }
    }

    void Fire() {
        if (Timer <= 0f) {
            GameObject obj = Instantiate(harpoonPrefab, transform.position, transform.rotation);
            sprite.enabled = false;
            var harpoon = obj.GetComponent<Harpoon>();
            harpoon.Parent = parent;
            Timer = Cooldown; 
        }
    }

    void LookAt(Vector3 point) {

    }
}
