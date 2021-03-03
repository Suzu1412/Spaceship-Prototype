using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharController : MonoBehaviour, IHealth
{
    protected int _currentHealth;
    protected Rigidbody2D _rb;
    protected SpriteRenderer _sprite;
    protected BoxCollider2D _collider;
    protected ObjectPooler _objectPooler;
    [HideInInspector] protected ScoreManager score;
    protected bool _isDeath;
    protected GameManager _manager;

    public int currentHealth { get { return _currentHealth; } }
    public bool isDeath { get { return _isDeath; } }
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
    }

    public abstract void Damage(int amount);

    public abstract void Death();

    public abstract void Heal(int amount);

    public void SetScore(ScoreManager score)
    {
        this.score = score;
    }
    public ScoreManager GetScore()
    {
        return score;
    }
}
