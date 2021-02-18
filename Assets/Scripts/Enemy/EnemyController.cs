using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour, IHealth
{
    [SerializeField] private int currentHealth;
    [SerializeField] public EnemyStats stats;
    [SerializeField] public FiniteStateMachine finiteStateMachine;
    public Rigidbody2D rb { get; private set; }
    [HideInInspector] public Transform playerPosition;
    [HideInInspector] public float direction;
    [HideInInspector] public bool targetFailed;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = stats.maxHealth;
        direction = -1f;
    }

    void OnEnable()
    {
        finiteStateMachine.Enter(this);
    }

    void OnDisable()
    {
        finiteStateMachine.Exit(this);
    }

    // Update is called once per frame
    void Update()
    {
        finiteStateMachine.LogicUpdate(this);
    }
    
    void FixedUpdate()
    {
        finiteStateMachine.PhysicsUpdate(this);
    }

    
    #region IHealth Implementation
    public void Heal(int amount)
    {

        if (currentHealth + amount >= stats.maxHealth)
        {
            currentHealth = stats.maxHealth;
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
        //impactEffect.SpawnFromPool(this.name, this.transform.position, Quaternion.identity);
        //GameObject explosion = explosionEffect.SpawnFromPool(this.tag, this.transform.position, Quaternion.identity);
        //explosion.GetComponent<ParticleSystem>().Play();

        this.gameObject.SetActive(false);
    }
    #endregion

    private void FindClosestEnemy()
    {
        float distanceToClosestEnemy = Mathf.Infinity;
        GameObject closestEnemy = null;
        GameObject[] allEnemies = DetectEnemy();

        if (allEnemies.Length > 0)
        {
            foreach (GameObject currentEnemy in allEnemies)
            {
                float distanceToEnemy = (currentEnemy.transform.position - this.transform.position).sqrMagnitude;

                if (distanceToEnemy < distanceToClosestEnemy)
                {
                    distanceToClosestEnemy = distanceToEnemy;
                    closestEnemy = currentEnemy;
                }
            }
            playerPosition = closestEnemy.transform;
            Debug.DrawLine(this.transform.position, closestEnemy.transform.position);
        }
        else
        {
            playerPosition = this.transform;
        }
    }

    private GameObject[] DetectEnemy()
    {
        return GameObject.FindGameObjectsWithTag("Player");
    }

    public void InvokeFindClosestEnemy()
    {
        InvokeRepeating("FindClosestEnemy", 0, 0.3f);
    }

    public void DisableFindClosestEnemy()
    {
        CancelInvoke("FindClosestEnemy");
    }

    public Vector3 EnemyDirection()
    {
        Vector3 direction = playerPosition.transform.position - transform.position;
        return direction;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            collision.collider.GetComponent<IHealth>().Damage(stats.collissionDamage);
            Death();
        }
    }
}
