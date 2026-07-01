using UnityEngine;

public class HeroKnight : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float m_speed = 6.0f;
    [SerializeField] float m_jumpForce = 14.0f;
    [SerializeField] float m_rollForce = 8.0f;

    [Header("Attack")]
    [SerializeField] Transform attackPoint;
    [SerializeField] float attackRange = 1.1f;
    [SerializeField] LayerMask enemyLayers;
    [SerializeField] int attackDamage = 30;

    [Header("Health")]
    [SerializeField] float m_maxHealth = 100f;

    [Header("Sounds")]
    public AudioClip jumpSound;
    public AudioClip attackSound;
    public AudioClip hurtSound;
    public AudioClip deathSound;
    public AudioClip rollSound;

    private Animator m_animator;
    private Rigidbody2D m_body2d;
    private Sensor_HeroKnight m_groundSensor;

    private float m_currentHealth;
    private bool m_grounded = false;
    private bool m_rolling = false;
    private bool m_isBlocking = false;
    private bool m_isInvulnerable = false;
    private float m_invulnerableTime = 0f;
    private bool isDead = false;

    private int m_facingDirection = 1;
    private int m_currentAttack = 0;
    private float m_timeSinceAttack = 0f;

    private AudioManager audioManager;

    void Start()
    {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_currentHealth = m_maxHealth;

        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
        audioManager = AudioManager.instance;
    }

    void Update()
    {
        if (isDead) return;

        m_timeSinceAttack += Time.deltaTime;

        if (m_isInvulnerable)
        {
            m_invulnerableTime -= Time.deltaTime;
            if (m_invulnerableTime <= 0) m_isInvulnerable = false;
        }

        m_grounded = m_groundSensor.State();
        m_animator.SetBool("Grounded", m_grounded);
        m_animator.SetFloat("AirSpeedY", m_body2d.linearVelocity.y);

        if (m_grounded)
            m_animator.ResetTrigger("Jump");

        float inputX = Input.GetAxis("Horizontal");

        if (inputX > 0.01f)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            m_facingDirection = 1;
        }
        else if (inputX < -0.01f)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            m_facingDirection = -1;
        }

        m_isBlocking = Input.GetMouseButton(1);
        m_animator.SetBool("IdleBlock", m_isBlocking);

        if (m_rolling)
        {
            if (!m_animator.GetCurrentAnimatorStateInfo(0).IsName("Roll"))
                m_rolling = false;
        }

        if (!m_rolling && !m_isBlocking)
            m_body2d.linearVelocity = new Vector2(inputX * m_speed, m_body2d.linearVelocity.y);

        HandleInput();
        UpdateAnimation(inputX);
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0) && m_timeSinceAttack > 0.3f && !m_rolling)
        {
            Attack();
            m_timeSinceAttack = 0f;
            if (audioManager != null && attackSound != null)
                audioManager.PlaySound(attackSound, 1f);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && !m_rolling)
        {
            m_rolling = true;
            m_animator.SetTrigger("Roll");
            m_body2d.linearVelocity = new Vector2(m_facingDirection * m_rollForce, m_body2d.linearVelocity.y);
            if (audioManager != null && rollSound != null)
                audioManager.PlaySound(rollSound, 0.9f);
        }

        if (Input.GetKeyDown(KeyCode.Space) && m_grounded && !m_rolling)
        {
            m_animator.ResetTrigger("Jump");
            m_animator.SetTrigger("Jump");
            m_grounded = false;
            m_animator.SetBool("Grounded", false);
            m_body2d.linearVelocity = new Vector2(m_body2d.linearVelocity.x, m_jumpForce);
            m_groundSensor.Disable(0.4f);

            if (audioManager != null && jumpSound != null)
                audioManager.PlaySound(jumpSound, 1f);
        }
    }

    void Attack()
    {
        m_animator.SetTrigger("Attack" + ((m_currentAttack % 3) + 1));
        m_currentAttack++;

        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider2D hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;
            hit.GetComponent<EvilWizardAI>()?.TakeDamage(attackDamage);
            hit.GetComponent<BringerOfDeath>()?.TakeDamage(attackDamage);
        }
    }

    void UpdateAnimation(float inputX)
    {
        m_animator.SetInteger("AnimState", Mathf.Abs(inputX) > 0.01f ? 1 : 0);
    }

    public void TakeDamage(float damage)
    {
        if (isDead || m_isInvulnerable || m_rolling || m_isBlocking) return;

        m_currentHealth -= damage;
        m_animator.SetTrigger("Hurt");
        m_isInvulnerable = true;
        m_invulnerableTime = 0.6f;

        if (audioManager != null && hurtSound != null)
            audioManager.PlaySound(hurtSound, 1f);

        if (m_currentHealth <= 0)
            Die();
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        m_animator.SetTrigger("Death");
        m_body2d.linearVelocity = Vector2.zero;
        m_body2d.bodyType = RigidbodyType2D.Static;

        if (audioManager != null && deathSound != null)
            audioManager.PlaySound(deathSound, 1f);

        Invoke(nameof(OpenLosePage), 1.5f);
    }

    private void OpenLosePage()
    {
        if (UIManager.instance != null)
        {
            UIManager.instance.GoToPage(3);
            Debug.Log("✅ GoToPage(3) вызван");
        }
        else
        {
            Debug.LogError("UIManager.instance == null!");
        }
    }

    public void AddDamage(int amount) { attackDamage += amount; }
    public void AddSpeed(float amount) { m_speed += amount; }
    public void AddJump(float amount) { m_jumpForce += amount; }
    public float GetHP() => m_currentHealth;
    public float GetMaxHP() => m_maxHealth;
}