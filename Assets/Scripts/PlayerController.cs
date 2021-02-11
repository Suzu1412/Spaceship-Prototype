using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IHealth
{
    private InputController input;
    private Rigidbody2D rb;
    private ObjectPooler objectPooler;

    [Header("Player Stats")]
    public int maxHealth = 1;
    public int currentHealth;
    public float moveSpeed = 3f;
    [Range(0, 3)]
    public int currentLevel;// { get; private set; }
    [Range(1, 64)] public int power;

    [Header("Movement Constraint")]
    [SerializeField] private Transform bottomLeftCorner;
    [SerializeField] private Transform topRightCorner;
    [SerializeField] private float offset;

    private void Awake()
    {
        input = GetComponent<InputController>();
        rb = GetComponent<Rigidbody2D>();
        objectPooler = ObjectPooler.Instance;

        if (input == null) Debug.LogError("InputController not attached to player");
        if (rb == null) Debug.LogError("Rigidbody2D not attached to player");
    }

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        power = 1;
        
        if (bottomLeftCorner == null) bottomLeftCorner = GameObject.Find("bottomLeftCorner").transform;
        if (topRightCorner == null) topRightCorner = GameObject.Find("topRightCorner").transform;
    }

    // Update is called once per frame
    void Update()
    {
        RestrictMovement();
    }

    private void FixedUpdate()
    {
        Move();
        //WeaponLevel();
    }

    private void Move()
    {
        Vector2 movement = new Vector2(input.horizontal * moveSpeed, input.vertical * moveSpeed);
        rb.velocity = movement;
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
    public void Heal(int amount)
    {
        if (currentHealth + amount > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else
        {
            currentHealth += amount;
        }
    }

    public void Damage(int amount)
    {
        if (currentHealth - amount <= 0)
        {
            Death();
        }
        else
        {
            currentHealth -= amount;
        }
    }

    public void Death()
    {
        Debug.Log("Player Death");
    }
    #endregion

    public ObjectPooler GetObjectPooler() 
    {
        return objectPooler;
    }

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
