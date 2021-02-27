using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CharController
{
    private InputController _input;
    
    [Header("Player Attributes")]
    [SerializeField] private PlayerStats _stats;
    [Range(0, 3)] public int currentLevel;
    [Range(1, 64)] public int power;

    [Header("Decision Making")]
    private bool _canShoot = false;

    [Header("Movement Constraint")]
    [SerializeField] private Transform bottomLeftCorner;
    [SerializeField] private Transform topRightCorner;
    [SerializeField] private float offset;

    public InputController input { get { return _input; } }
    public bool canShoot { get { return _canShoot; } }
    public PlayerStats stats { get { return _stats; } }

    protected override void Awake()
    {
        base.Awake();
        _input = GetComponent<InputController>();
        if (_input == null) Debug.LogError(this.gameObject.name + " missing InputController");
        if (_stats == null) Debug.Log(this.gameObject.name + " missing Stats");
        Invoke("CanShoot", 1f);
    }

    void CanShoot()
    {
        _canShoot = true;
    }

    // Start is called before the first frame update
    protected void Start()
    {
        power = 1;
        
        if (bottomLeftCorner == null) bottomLeftCorner = GameObject.Find("bottomLeftCorner").transform;
        if (topRightCorner == null) topRightCorner = GameObject.Find("topRightCorner").transform;
        if (offset == 0) offset = 0.2f;
    }

    void OnEnable()
    {
        _currentHealth = stats.maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        RestrictMovement();
        /*
        _canShoot = false;
        if (IsShooting())
        {
            _canShoot = true;
        }
        */
    }

    private void FixedUpdate()
    {
        Move();
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
        if (_currentHealth - amount <= 0)
        {
            Death();
        }
        else
        {
            _currentHealth -= amount;
        }
    }

    public override void Death()
    {
        this.gameObject.SetActive(false);
    }
    #endregion

    public bool IsShooting()
    {
        return input.isShooting;
    }

    private void WeaponLevel()
    {
        if (power >= 1 && power <= 8)
        {
            currentLevel = 0;
        }
        else if (power >= 9 && power <= 16)
        {
            currentLevel = 1;
        }
        else if (power >= 17 && power <= 32)
        {
            currentLevel = 2;
        }
        else if (power >= 33 && power <= 64)
        {
            currentLevel = 3;
        }
    }
}
