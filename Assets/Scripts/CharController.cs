using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharController : MonoBehaviour, IHealth
{
    protected int _currentHealth;
    protected Rigidbody2D _rb;
    protected ObjectPooler _objectPooler;
    [HideInInspector] protected ScoreManager score;
    protected bool isDeath;
    protected GameManager _manager;

    public int currentHealth { get { return _currentHealth; } }
    public Rigidbody2D rb { get { return _rb; } }
    public ObjectPooler objectPooler { get { return _objectPooler; } }

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _objectPooler = ObjectPooler.Instance;
        if (rb == null) Debug.Log(this.gameObject.name + " missing RigidBody2D");
        _manager = GameObject.FindObjectOfType<GameManager>();

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
