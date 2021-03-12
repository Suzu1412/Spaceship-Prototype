using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharController : MonoBehaviour, IHealth
{
    protected int _currentHealth;
    protected int _maxHealth;
    protected bool _healed;
    protected bool _damaged;
    protected bool _isDeath;
    protected bool _invulnerable;
    protected Rigidbody2D _rb;
    [SerializeField] protected SpriteRenderer _sprite;
    protected BoxCollider2D _collider;
    protected ObjectPooler _objectPooler;
    [HideInInspector] protected ScoreManager score;
    [HideInInspector] protected ScoreManager highScore;
    protected GameManager _manager;
    protected HealthBarManager _healthBar;

    public int currentHealth { get { return _currentHealth; } }
    public int maxHealth { get { return _maxHealth; } }
    public bool isHealed { get { return _healed; } }
    public bool isDamaged { get { return _damaged; } }
    public bool isDeath { get { return _isDeath; } }
    public bool isInvulnerable { get { return _invulnerable; } }
    public Rigidbody2D rb { get { return _rb; } }
    public ObjectPooler objectPooler { get { return _objectPooler; } }

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<BoxCollider2D>();
        _sprite = GetComponentInChildren<SpriteRenderer>();
        _objectPooler = ObjectPooler.Instance;
        if (rb == null) Debug.Log(this.gameObject.name + " missing RigidBody2D");
        _manager = GameObject.FindObjectOfType<GameManager>();
    }

    protected virtual void OnEnable()
    {
        _collider.enabled = true;
        _sprite.enabled = true;
        _isDeath = false;
    }

    public abstract void Damage(int amount);

    public abstract void Death();

    public abstract void Heal(int amount);

    public void SetScore(ScoreManager score)
    {
        this.score = score;
    }

    public void SetHighScore(ScoreManager highScore)
    {
        this.highScore = highScore;
    }

    public void SetHealedFalse()
    {
        _healed = false;
    }

    public void SetDamageFalse()
    {
        _damaged = false;
    }

    public void SetHealthBar(HealthBarManager manager)
    {
        _healthBar = manager;
    }

    protected void SetActiveFalse()
    {
        this.gameObject.SetActive(false);
    }
}
