using UnityEngine;

public class Projectile : MonoBehaviour
{
    [field: Header("Stats")]
    [field: SerializeField] public int ATK { get; private set; } = 20;
    [field: SerializeField] public int SPD { get; private set; } = 10;

    [Header("Projectile Properties")]
    [SerializeField] int lifetime = 3;

    public void SetATK(int attack) => ATK = attack;
    public void SetSPD(int speed) => SPD = speed;

    void Move()
    {
        transform.position += transform.up * SPD * Time.fixedDeltaTime;
    }
    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void FixedUpdate()
    {
        Move();
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Enemy"))
        {
            var enemy = other.gameObject.GetComponent<Enemy>();
            if (enemy.IsAlive()) enemy.TakeDamage(ATK);
            Destroy(gameObject);
        }
    }
}
