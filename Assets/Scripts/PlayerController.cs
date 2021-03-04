using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : CharController
{
    [Header("Player variables")]
    
    private InputController _input;
    private CircleCollider2D _magnetCollider;
    private bool _victory;

    [Header("Player Start Animation")]
    public float smoothTimeStart = 0.6F;
    public float smoothTimeEnd = 0.6F;
    public float smoothVictory = 1.2f;
    bool arrivedAtStartPosition;
    private Vector3 startMarker;
    private Vector3 endMarker;
    private Vector3 velocity = Vector3.zero;

    [Header("Player Attributes")]
    [SerializeField] private PlayerStats _stats;
    private int _currentLevel;

    [Header("Decision Making")]
    private bool _canShoot = false;

    [Header("Movement Constraint")]
    [SerializeField] private Transform bottomLeftCorner;
    [SerializeField] private Transform topRightCorner;
    [SerializeField] private float offset;

    public InputController input { get { return _input; } }
    public bool canShoot { get { return _canShoot; } }
    public PlayerStats stats { get { return _stats; } }
    public int currentLevel { get { return _currentLevel; } }
    public bool victory { get { return _victory; } }

    protected override void Awake()
    {
        base.Awake();
        _input = GetComponent<InputController>();
        _magnetCollider = GetComponentInChildren<CircleCollider2D>();
        if (_input == null) Debug.LogError(this.gameObject.name + " missing InputController");
        if (_stats == null) Debug.Log(this.gameObject.name + " missing Stats");
        _victory = false;
    }

    public void Shoot()
    {
        _canShoot = true;
    }

    public void CanShoot(bool canShoot)
    {
        _canShoot = canShoot;
    }

    // Start is called before the first frame update
    protected void Start()
    {
        if (bottomLeftCorner == null) bottomLeftCorner = GameObject.Find("bottomLeftCorner").transform;
        if (topRightCorner == null) topRightCorner = GameObject.Find("topRightCorner").transform;
        if (offset == 0) offset = 0.2f;
        arrivedAtStartPosition = false;
        startMarker = new Vector3(this.transform.position.x, -1.5f, 0f);
        endMarker = new Vector3(this.transform.position.x, -3.2f, 0f);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _maxHealth = stats.maxHealth;
        _currentHealth = _maxHealth;
        float fillAmount = (float)currentHealth / (float)stats.maxHealth;
        _isDeath = false;
        _invulnerable = false;
    }

    // Update is called once per frame
    void Update()
    {
        switch(_manager.state){
            case GameState.Start:
                MoveToStartPosition();
                break;

            case GameState.Playing:
                RestrictMovement();
                break;

            case GameState.Victory:
                Victory();
                break;
        }
    }

    private void FixedUpdate()
    {
        if (_manager.state == GameState.Playing)
        {
            Move();
        }
    }

    private void Move()
    {
        Vector2 movement = new Vector2(input.horizontal * stats.moveSpeed, input.vertical * stats.moveSpeed);
        _rb.velocity = movement;
    }

    private void RestrictMovement()
    {
        if (transform.position.x <= bottomLeftCorner.position.x + offset) 
            transform.position = new Vector3(bottomLeftCorner.position.x + offset, transform.position.y);

        if (transform.position.x >= topRightCorner.position.x - offset)
            transform.position = new Vector3(topRightCorner.position.x - offset, transform.position.y);

        if (transform.position.y <= bottomLeftCorner.position.y + offset)
            transform.position = new Vector3(transform.position.x, bottomLeftCorner.position.y + offset);

        if (transform.position.y >= topRightCorner.position.y - offset)
            transform.position = new Vector3(transform.position.x, topRightCorner.position.y - offset);
    }

    #region Implementing Ihealth
    public override void Heal(int amount)
    {
        if (isDeath) return;

        _healthBar.ResetHeal();
        _healed = true;
        if (_currentHealth + amount > stats.maxHealth)
        {
            _currentHealth = stats.maxHealth;
        }
        else
        {
            _currentHealth += amount;
        }
    }

    public override void Damage(int amount)
    {
        if (isDeath) return;
        if (isInvulnerable) return;

        _healthBar.ResetDamage();
        _damaged = true;
        
        if (_currentHealth - amount <= 0)
        {
            _currentHealth = 0;
            Death();
        }
        else
            {
            Invoke("Invulnerability", 0f);
            Invoke("RemoveInvulnerability", 1.5f);
            _currentHealth -= amount;
        }
    }

    public override void Death()
    {
        if (!isDeath)
        {
            _canShoot = false;
            _collider.enabled = false;
            _sprite.enabled = false;
            _isDeath = true;
            Invoke("SetActiveFalse", 1f);
        }
    }
    #endregion

    public bool IsShooting()
    {
        return input.isShooting;
    }

    void MoveToStartPosition()
    {
        if (!arrivedAtStartPosition)
        {
            Vector3 targetPosition = startMarker;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTimeStart);
            if (Vector3.Distance(transform.position, startMarker) < 0.2f) arrivedAtStartPosition = true;
        }
        else
        {
            Vector3 targetPosition = endMarker;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTimeEnd);
        }
    }

    #region LevelUp
    public void AddPower(int amount)
    {
        int newLevel = _stats.AddExperience(amount);

        if (_currentLevel != newLevel)
        {
            LevelUp();
        }
        _currentLevel = newLevel;
    }

    void LevelUp()
    {
        Debug.Log("Level Up");
        //Instanciar efecto de Level Up
        //Hacer aparecer texto de Level Up sobre jugador
    }
    #endregion

    void Victory()
    {
        _invulnerable = true;
        _magnetCollider.radius = 10f;
        _victory = true;
        rb.velocity = Vector2.up * 5f;
    }

    void Invulnerability()
    {
        _invulnerable = true;
        _sprite.color = new Color(_sprite.color.r, _sprite.color.g, _sprite.color.b, 0.3f);
    }

    void RemoveInvulnerability()
    {
        _invulnerable = false;
        _sprite.color = new Color(_sprite.color.r, _sprite.color.g, _sprite.color.b, 1f);
    }

    public void AddScore(int amount)
    {
        score.SetScore(amount);

        if (highScore.GetScore() <= score.GetScore())
        {
            highScore.SetScore(score.GetScore() - highScore.GetScore());
            PlayerPrefs.SetInt("HighScore", score.GetScore());
        }
    }
}
