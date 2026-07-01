using UnityEngine;
using System.Collections;

public class BringerOfDeath : MonoBehaviour, IEnemy
{
    [Header("Stats")]
    public float maxHealth = 200f;
    int IEnemy.maxHealth => (int)maxHealth;
    public float currentHealth;
    public float moveSpeed = 2f;
    public float chaseSpeed = 3.5f;

    [Header("Attack Settings")]
    public float attackDamage = 20f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1.5f;

    [Header("Teleport Cast")]
    public float spellRange = 8f;
    public float spellCooldown = 6f;
    public float teleportHeight = 3.8f;

    [Header("Detection")]
    public float detectionRange = 10f;

    [Header("References")]
    public Transform attackPoint;

    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private Transform player;
    private float lastAttackTime;
    private float lastSpellTime;
    private bool isDead = false;
    private bool isHurt = false;
    private bool isAttacking = false;
    private bool isCasting = false;

    private AudioManager audioManager;

    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int CastHash = Animator.StringToHash("Cast");
    private static readonly int HurtHash = Animator.StringToHash("Hurt");
    private static readonly int DeadHash = Animator.StringToHash("Dead");

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
    }

    private void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;

        audioManager = AudioManager.instance;
    }

    private void Update()
    {
        if (isDead || isHurt || isAttacking || isCasting || player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);
        animator.SetBool("isGrounded", Mathf.Abs(rb.linearVelocity.y) < 0.1f);
        animator.SetFloat("JumpSpeed", rb.linearVelocity.y);

        if (dist <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            StartCoroutine(DoAttack());
        }
        else if (dist <= spellRange && Time.time >= lastSpellTime + spellCooldown)
        {
            StartCoroutine(DoCast());
        }
        else if (dist <= detectionRange)
        {
            ChasePlayer();
        }
        else
        {
            Idle();
        }
    }

    private void Idle()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        animator.SetFloat(SpeedHash, 0f);
    }

    private void ChasePlayer()
    {
        float dir = player.position.x - transform.position.x;
        rb.linearVelocity = new Vector2(Mathf.Sign(dir) * chaseSpeed, rb.linearVelocity.y);
        animator.SetFloat(SpeedHash, Mathf.Abs(rb.linearVelocity.x));
        spriteRenderer.flipX = dir < 0;
    }

    private IEnumerator DoAttack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        rb.linearVelocity = Vector2.zero;
        spriteRenderer.flipX = player.position.x < transform.position.x;
        animator.SetTrigger(AttackHash);

        if (audioManager != null && audioManager.bringerAttack != null)
            audioManager.PlaySound(audioManager.bringerAttack);

        yield return new WaitForSeconds(0.4f);
        DealMeleeDamage();
        yield return new WaitForSeconds(0.6f);
        isAttacking = false;
    }

    private void DealMeleeDamage()
    {
        if (attackPoint == null) return;
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<HeroKnight>(out var hero))
                hero.TakeDamage(attackDamage);
        }
    }

    private IEnumerator DoCast()
    {
        isCasting = true;
        lastSpellTime = Time.time;
        rb.linearVelocity = Vector2.zero;
        spriteRenderer.flipX = player.position.x < transform.position.x;
        animator.SetTrigger(CastHash);

        if (audioManager != null && audioManager.bringerCast != null)
            audioManager.PlaySound(audioManager.bringerCast);

        yield return new WaitForSeconds(0.4f);
        TeleportAbovePlayer();
        yield return new WaitForSeconds(0.6f);
        isCasting = false;
    }

    private void TeleportAbovePlayer()
    {
        if (player == null) return;

        Vector3 newPos = new Vector3(player.position.x, player.position.y + teleportHeight, transform.position.z);
        transform.position = newPos;

        if (audioManager != null && audioManager.bringerTeleport != null)
            audioManager.PlaySound(audioManager.bringerTeleport, 0.95f);
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;
        currentHealth -= damage;

        GetComponent<EnemyHealthBar>()?.SetHealth(currentHealth);

        if (audioManager != null && audioManager.bringerHurt != null)
            audioManager.PlaySound(audioManager.bringerHurt, 1f);

        if (currentHealth <= 0)
            Die();
        else
            StartCoroutine(HurtRoutine());
    }

    private IEnumerator HurtRoutine()
    {
        isHurt = true;
        rb.linearVelocity = Vector2.zero;
        animator.SetTrigger(HurtHash);
        yield return new WaitForSeconds(0.5f);
        isHurt = false;
    }

    private void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        animator.SetTrigger(DeadHash);

        if (audioManager != null && audioManager.bringerDeath != null)
            audioManager.PlaySound(audioManager.bringerDeath, 1f);

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Destroy(gameObject, 1.2f);
    }
}