using UnityEngine;

public class EvilWizardAI : MonoBehaviour , IEnemy
{
    [Header("Health")]
    public int maxHealth = 80;
    int IEnemy.maxHealth => maxHealth;
    private int currentHealth;

    [Header("Movement")]
    public float moveSpeed = 4f;
    public float detectionRange = 10f;
    public float attackRange = 2.4f;

    [Header("Attack")]
    public float attackCooldown = 1.2f;
    public int damage = 25;

    [Header("References")]
    public Collider2D attackHitbox;

    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Transform player;
    private HeroKnight heroKnight;

    private float attackTimer = 0f;
    private bool isDead = false;
    private bool isAttacking = false;

    private AudioManager audioManager;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p)
        {
            player = p.transform;
            heroKnight = p.GetComponent<HeroKnight>();
        }

        audioManager = AudioManager.instance;
        if (attackHitbox) attackHitbox.enabled = true;
    }

    void Update()
    {
        if (isDead || player == null) return;

        anim.SetBool("isGrounded", Mathf.Abs(rb.linearVelocity.y) < 0.1f);
        anim.SetFloat("JumpSpeed", rb.linearVelocity.y);
        attackTimer += Time.deltaTime;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= attackRange)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            Flip(player.position.x > transform.position.x ? 1 : -1);
            anim.SetInteger("AnimState", 0);

            if (attackTimer >= attackCooldown)
            {
                attackTimer = 0f;
                isAttacking = true;
                anim.SetTrigger("Attack1");

                // Наносим урон напрямую
                if (heroKnight != null)
                    heroKnight.TakeDamage(damage);

                if (audioManager != null && audioManager.wizardAttack != null)
                    audioManager.PlaySound(audioManager.wizardAttack, 0.9f);

                // Сбрасываем флаг атаки через задержку
                Invoke(nameof(ResetAttack), 0.5f);
            }
        }
        else if (dist <= detectionRange)
        {
            float dir = player.position.x > transform.position.x ? 1f : -1f;
            rb.linearVelocity = new Vector2(dir * moveSpeed, rb.linearVelocity.y);
            Flip(dir);
            anim.SetInteger("AnimState", 1);
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            anim.SetInteger("AnimState", 0);
        }
    }

    void ResetAttack() => isAttacking = false;

    void Flip(float dir) => sr.flipX = dir < 0;

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        currentHealth -= damage;

        GetComponent<EnemyHealthBar>()?.SetHealth(currentHealth);

        if (audioManager != null && audioManager.wizardHurt != null)
            audioManager.PlaySound(audioManager.wizardHurt, 1f);

        anim.SetTrigger("Hurt");

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;
        anim.SetTrigger("Death");

        if (audioManager != null && audioManager.wizardDeath != null)
            audioManager.PlaySound(audioManager.wizardDeath, 1f);

        Destroy(gameObject, 1.2f);
    }
}