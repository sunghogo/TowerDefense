using UnityEngine;

public class Unit : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer spriteRenderer;

    public void Attack()
    {
        animator.SetTrigger("Attack");
    }

    public void Idle()
    {
        animator.SetTrigger("Idle");
    }
    
    void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
    }
}
