using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] GameObject damageTextPrefab;
    [SerializeField] GameObject goldTextPrefab;
    [SerializeField] RectTransform enemyCanvas;
    [SerializeField] Bar HPBar;
    [SerializeField] TextTMP HPText;
    [SerializeField] TextTMP LVLText;

    [Header("Components")]
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Rigidbody2D rb;



    [field: Header("Enemy Name")]
    [field: SerializeField] public string EnemyName { get; protected set; }

    [Header("Starting Stats")]
    [SerializeField] protected int startingLVL = 1;
    [SerializeField] protected int startingMaxHP = 10;
    [SerializeField] protected int startingGoldDrop = 1;
    [SerializeField] protected int startingATK = 1;
    [SerializeField] protected int startingDEF = 0;
    [SerializeField] protected float startingSPD = 1;

    [field: Header("Current Stats")]
    [field: SerializeField] public int LVL { get; protected set; }
    [field: SerializeField] public int MaxHP { get; protected set; }
    [field: SerializeField] public int HP { get; protected set; }
    [field: SerializeField] public int GoldDrop { get; protected set; }
    [field: SerializeField] public int ATK { get; protected set; }
    [field: SerializeField] public int DEF { get; protected set; }
    [field: SerializeField] public float SPD { get; protected set; }

    [field: Header("Navigation")]
    [field: SerializeField] public List<PathTile> Path { get; protected set; }

    [field: Header("State")]
    [field: SerializeField] public bool IsMoving { get; protected set; } = false;
    [field: SerializeField] public bool PathFound { get; protected set; } = false;

    Vector2 direction = Vector2.zero;

    public IEnumerator MoveTiles(PathTile startTile, PathTile endTile)
    {
        IsMoving = true;

        Vector3 startPos = startTile.position;
        Vector3 endPos   = endTile.position;

        direction = (endPos - startPos).normalized;
        ProcessAnimation();

        float elapsed  = 0f;
        float duration = Vector3.Distance(startPos, endPos) / SPD;

        while (elapsed < duration && IsAlive())
        {
            elapsed += Time.fixedDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            Vector3 newPos = Vector3.Lerp(startPos, endPos, t);
            rb.MovePosition(newPos);

            yield return new WaitForFixedUpdate();
        }

        if (IsAlive())
        {
            rb.MovePosition(endPos);
            IsMoving = false;

            if (endTile == PathTilemap.Instance.EndTile)
            {
                GameManager.Instance.DecreaseHealth(ATK);
                Destroy(gameObject);
            }
        }
    }

    public bool IsAlive() => HP > 0;

    public void TakeDamage(int attackerATK)
    {
        int damage = Mathf.Max(attackerATK - DEF, 0);
        var damageText = Instantiate(damageTextPrefab, enemyCanvas).GetComponent<FloatingText>();
        damageText.SetText($"-{damage}");
        HP = Mathf.Max(HP - damage, 0);
        UpdateHpText();
        UpdateHpBar();
        if (HP <= 0) ProcessDeath();
    }

    public void LevelUp(int levels)
    {
            LVL = startingLVL + levels;
            GoldDrop = startingGoldDrop + levels;
            HP = MaxHP = startingMaxHP + 5 * levels;
            SPD = startingSPD + 0.1f * levels;
    }

    protected void InitializeStats()
    {
        LVL = startingLVL;
        HP = MaxHP = startingMaxHP;
        GoldDrop = startingGoldDrop;
        ATK = startingATK;
        DEF = startingDEF;
        SPD = startingSPD;
    }

    void UpdateHpText()
    {
        string tag = HPText.gameObject.tag;
        string val = $"{tag}: {HP}";
        HPText.SetText(val);
    }

    void UpdateLVLText()
    {
        string tag = LVLText.gameObject.tag;
        string val = $"{tag} {LVL} {EnemyName}";
        LVLText.SetText(val);
    }

    void UpdateHpBar()
    {
        HPBar.SetValue(HP, MaxHP);
    }

    void DeactivateUI()
    {
        HPBar.gameObject.SetActive(false);
        LVLText.gameObject.SetActive(false);
        HPText.gameObject.SetActive(false);
    }

    void ProcessMovement()
    {
        if (!IsMoving && Path != null && Path.Count > 1) {
            PathTile currentTile = Path[0];
            PathTile nextTile = Path[1];
            Path.RemoveAt(0);
            StartCoroutine(MoveTiles(currentTile, nextTile));
        }
    }

    void ProcessAnimation()
    {
        animator.SetFloat("x", direction.x);
        animator.SetFloat("y", direction.y);
        if (direction.x >= 0) spriteRenderer.flipX = true;
        else if (direction.x < 0) spriteRenderer.flipX = false;
    }

    void ProcessDeath()
    {
        animator.SetTrigger("Death");
        DeactivateUI();
        var goldText = Instantiate(goldTextPrefab, enemyCanvas).GetComponent<FloatingText>();
        goldText.SetText($"+{GoldDrop}");
        GameManager.Instance.IncreaseGold(GoldDrop);
        GameManager.Instance.IncrementScore();
        Destroy(gameObject, 1f);
    }

    void SetShortestPath()
    {
        if (PathFound) return;

        Vector3Int cell = PathTilemap.Instance.Tilemap.WorldToCell(transform.position);
        PathTile startingTile = PathTilemap.Instance.GetPathTile(cell);

        if (startingTile != null)
        {
            Path = PathTilemap.Instance.FindShortestPath(startingTile, PathTilemap.Instance.EndTile);
            PathFound = true;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other is TilemapCollider2D)
        {
            SetShortestPath();
        }
    }

    void Awake()
    {
        InitializeStats();
        OnAwake();

        GameManager.OnGameOver += HandleGameOver;
    }

    void OnDestroy()
    {
        GameManager.Instance.RemoveEnemy(this);

        GameManager.OnGameOver -= HandleGameOver;
    }

    void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (rb == null) rb = GetComponent<Rigidbody2D>();


        UpdateHpText();
        UpdateHpBar();
        UpdateLVLText();
    }

    void FixedUpdate()
    {
        ProcessMovement();
        OnFixedUpdate();
    }

    void HandleGameOver()
    {
        Destroy(gameObject);
    }

    protected virtual void OnAwake() { }
    protected virtual void OnFixedUpdate() { }
    
}
