using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Slider slider;
    public Vector3 offset;
    public float detectionRadius = 5f;
    public float hideDelay = 3f;

    [Header("Цвета")]
    public Color colorFull = new Color(0.30f, 0.85f, 0.30f);
    public Color colorMid = new Color(1.00f, 0.76f, 0.03f);
    public Color colorLow = new Color(0.89f, 0.29f, 0.29f);

    private Image _fill;
    private Coroutine _hideCoroutine;
    private IEnemy _enemy;
    private Transform _player;

    void Awake()
    {
        _fill = slider.fillRect.GetComponent<Image>();
        _enemy = GetComponent<IEnemy>();

        slider.maxValue = _enemy.maxHealth;
        slider.value = _enemy.maxHealth;
        slider.gameObject.SetActive(false);
    }

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) _player = p.transform;
    }

    void LateUpdate()
    {
        // Canvas дочерний к врагу — двигается сам, только offset применяем к слайдеру
        slider.transform.parent.position = transform.position + offset;

        if (_player == null) return;
        float dist = Vector2.Distance(transform.position, _player.position);
        if (!slider.gameObject.activeSelf && dist <= detectionRadius)
            ShowBar();
    }

    public void SetHealth(float health)
    {
        slider.value = health;
        UpdateColor(health / _enemy.maxHealth);
        ShowBar();
    }

    private void ShowBar()
    {
        slider.gameObject.SetActive(true);
        if (_hideCoroutine != null) StopCoroutine(_hideCoroutine);
        _hideCoroutine = StartCoroutine(HideWhenFar());
    }

    private IEnumerator HideWhenFar()
    {
        yield return new WaitForSeconds(hideDelay);
        while (true)
        {
            if (_player == null) break;
            if (Vector2.Distance(transform.position, _player.position) > detectionRadius) break;
            yield return new WaitForSeconds(0.5f);
        }
        slider.gameObject.SetActive(false);
    }

    private void UpdateColor(float ratio)
    {
        if (_fill == null) return;
        if (ratio > 0.6f)
            _fill.color = Color.Lerp(colorMid, colorFull, (ratio - 0.6f) / 0.4f);
        else if (ratio > 0.3f)
            _fill.color = Color.Lerp(colorLow, colorMid, (ratio - 0.3f) / 0.3f);
        else
            _fill.color = colorLow;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}