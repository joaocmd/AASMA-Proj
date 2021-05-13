using UnityEngine;

public class Health : MonoBehaviour
{

    public float hp = 100;
    public Transform healthBar;

    private float initialSize;

    void Start()
    {
        initialSize = healthBar.localScale.x;
    }

    void Update()
    {
        healthBar.rotation = Quaternion.identity;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        hp = Mathf.Clamp(hp - 40, 0, 100);
        healthBar.localScale = new Vector3(initialSize * (hp / 100), healthBar.localScale.y, healthBar.localScale.z);
        if (hp <= 0)
        {
            GameObject.FindWithTag("GameManager").GetComponent<EnvironmentManager>().RemoveShip(gameObject);
            Destroy(gameObject);
        }
    }
}
