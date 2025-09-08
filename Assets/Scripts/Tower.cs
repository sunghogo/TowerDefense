using UnityEngine;
using System.Collections.Generic;

public class Tower : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Unit unit;

    [Header("Components")]
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] CircleCollider2D circleCollider;

    [Header("Starting Stats")]
    [SerializeField] protected int startingATK = 1;
    [SerializeField] protected int startingATKSPD = 1;
    [SerializeField] protected int startingATKRadius = 2;

    [field: Header("Current Stats")]
    [field: SerializeField] public int GoldCost { get; protected set; } = 10;
    [field: SerializeField] public int ATK { get; protected set; }
    [field: SerializeField] public int ATKSPD { get; protected set; }
    [field: SerializeField] public int ATKRadius { get; protected set; }

    [field: Header("State")]
    [field: SerializeField] public bool CanAttack { get; protected set; } = true;

    [field: Header("Enemies in Range")]
    [field: SerializeField] public List<Enemy> EnemiesInRange { get; protected set; } = new List<Enemy>();

    float secondsPerAttack;
    float attackTimer = 0f;

    protected void InitializeStats()
    {
        ATK = startingATK;
        ATKSPD = startingATKSPD;
        ATKRadius = startingATKRadius;
    }

    void SetAttackRadius()
    {
        circleCollider.radius = ATKRadius;
    }

    void Attack(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle - 90f);
        Instantiate(projectilePrefab, transform.position, rotation, transform);
    }

    void ProcessAttackTimer()
    {
        if (attackTimer >= secondsPerAttack)
        {
            CanAttack = true;
            attackTimer = 0f;
        }
        attackTimer += Time.fixedDeltaTime;
    }

    void ProcessEnemiesInRange()
    {
        EnemiesInRange.RemoveAll(enemy => enemy == null);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            var enemy = collision.gameObject.GetComponent<Enemy>();
            if (!EnemiesInRange.Contains(enemy)) EnemiesInRange.Add(enemy);
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (CanAttack && collision.gameObject.CompareTag("Enemy") && EnemiesInRange.Count > 0)
        {
            ProcessEnemiesInRange();
            Attack(EnemiesInRange[0].transform);
            unit.Attack();
            attackTimer = 0f;
            CanAttack = false;
        }
        else
        {
            unit.Idle();
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            var enemy = collision.gameObject.GetComponent<Enemy>();
            if (EnemiesInRange.Contains(enemy)) EnemiesInRange.Remove(enemy);
            if (EnemiesInRange.Count == 0) unit.Idle();
        }
    }

    void Awake()
    {
        InitializeStats();
        SetAttackRadius();
        secondsPerAttack = 1 / ATKSPD;

        GameManager.OnGameOver += HandleGameOver;
    }

    void OnDestroy()
    {
        GameManager.OnGameOver -= HandleGameOver;
    }

    void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (circleCollider == null) circleCollider = GetComponent<CircleCollider2D>();
    }

    void FixedUpdate()
    {
        ProcessAttackTimer();
    }
    
    void HandleGameOver()
    {
        Destroy(gameObject);
    }

}
