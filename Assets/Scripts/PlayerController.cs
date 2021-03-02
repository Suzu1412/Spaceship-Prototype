using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CharController
{
    [Header("Player variables")]
    private InputController _input;
    private UnityEngine.UI.Image healthBar;
    private BoxCollider2D _collider;

    [Header("Player Start Animation")]
    bool arrivedAtStartPosition;
    private Vector3 startMarker;
    private Vector3 endMarker;
    public float smoothTimeStart = 0.6F;
    public float smoothTimeEnd = 0.6F;
    public float smoothVictory = 1.2f;
    private Vector3 velocity = Vector3.zero;

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
        _collider = GetComponent<BoxCollider2D>();
        if (_input == null) Debug.LogError(this.gameObject.name + " missing InputController");
        if (_stats == null) Debug.Log(this.gameObject.name + " missing Stats");
        

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
        power = 0;
        
        if (bottomLeftCorner == null) bottomLeftCorner = GameObject.Find("bottomLeftCorner").transform;
        if (topRightCorner == null) topRightCorner = GameObject.Find("topRightCorner").transform;
        if (offset == 0) offset = 0.2f;
        arrivedAtStartPosition = false;
        startMarker = new Vector3(this.transform.position.x, -1.5f, 0f);
        endMarker = new Vector3(this.transform.position.x, -3.2f, 0f);
        FillHealthBar();
    }

    void OnEnable()
    {
        _currentHealth = stats.maxHealth;
        isDeath = false;
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

    void Victory()
    {
        _collider.enabled = false;
        rb.velocity = Vector2.up * 5f;
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
        FillHealthBar();
    }

    public override void Damage(int amount)
    {
        if (_currentHealth - amount <= 0)
        {
            _currentHealth = 0;
            Death();
        }
        else
        {
            _currentHealth -= amount;
        }
        FillHealthBar();
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

    void MoveToStartPosition()
    {
        if (!arrivedAtStartPosition)
        {
            Vector3 targetPosition = startMarker;

            // Smoothly move the camera towards that target position
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTimeStart);

            if (Vector3.Distance(transform.position, startMarker) < 0.2f) arrivedAtStartPosition = true;
        }
        else
        {
            Vector3 targetPosition = endMarker;

            // Smoothly move the camera towards that target position
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTimeEnd);
        }
    }

    void FillHealthBar()
    {
        float fillAmount = (float)currentHealth / (float)stats.maxHealth;
        healthBar.fillAmount = fillAmount;
    }

    public void SetHealhBar(UnityEngine.UI.Image image)
    {
        healthBar = image;
    }

    public void AddPower(int amount)
    {
        if (power + amount > 50)
        {
            power = 50;
        }
        else
        {
            power += amount;
        }

        PlayerLevel();
    }

    void PlayerLevel()
    {
        if (power >= 0 && power <= 4)
        {
            currentLevel = 0;
        }
        else if (power >= 5 && power <= 20)
        {
            if (currentLevel < 1)
            {
                LevelUp();
            }
            currentLevel = 1;
        }
        else if (power >= 21 && power <= 49)
        {
            if (currentLevel < 2)
            {
                LevelUp();
            }
            currentLevel = 2;
        }
        else if(power == 50)
        {
            if (currentLevel < 3)
            {
                LevelUp();
            }
            currentLevel = 3;
        }
    }

    void LevelUp()
    {
        Debug.Log("Level Up");
        //Instanciar efecto de Level Up
        //Hacer aparecer texto de Level Up sobre jugador
    }
}
