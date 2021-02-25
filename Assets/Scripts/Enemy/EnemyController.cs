using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShootDecision
{
    None, 
    Random,
    Interval,
    OnDetection
}

public class EnemyController : MonoBehaviour, IHealth
{
    public int currentHealth { get; private set; }
    [SerializeField] public EnemyStats stats;
    public Rigidbody2D rb { get; private set; }
    private ObjectPooler objectPooler;

    [HideInInspector] public Transform playerPosition;
    [SerializeField] private State initialState;
    public bool detected { get; private set; }
    public bool canShoot { get; private set; }
    private State currentState;

    [Header("Follow Path Attributes")]
    private Transform path;
    public Transform waypoints;
    [HideInInspector] public int currentWaypoint;
    [HideInInspector] public bool followPath;

    [Header("Shoot")]
    public ShootDecision shootDecision;
    [Tooltip("On Random Shoot Decision")] [Range(0f, 3f)] public float minRandomShoot = 1f;
    [Tooltip("On Random Shoot Decision")] [Range(0f, 4f)] public float maxRandomShoot = 5f;
    [Tooltip("On Interval Shoot Decision")] [Range(0f, 2f)] public float shootIntervalMaxWait = 1f;


    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        objectPooler = ObjectPooler.Instance;
        path = GameObject.Find("Path").transform;
    }

    void OnEnable()
    {
        currentHealth = stats.maxHealth;
        currentState = initialState;
        currentState.Enter(this);
    }

    void OnDisable()
    {
        currentState.Exit(this);
    }

    // Update is called once per frame
    void Update()
    {
        currentState.LogicUpdate(this);
    }
    
    void FixedUpdate()
    {
        currentState.PhysicsUpdate(this);
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

    #region Player Detection
    public void DetectPlayer()
    {
        detected = false;

        FindClosestPlayer();

        if (playerPosition != null)
        {
            Vector2 PlayerVector = playerPosition.position - transform.position;

            if (Vector3.Angle(PlayerVector.normalized, -transform.up) < stats.angle * 0.5f)
            {
                if (PlayerVector.magnitude < stats.distance)
                {
                    detected = true;
                }
            }
        }
    }

    private void FindClosestPlayer()
    {
        float distanceToClosestEnemy = Mathf.Infinity;
        GameObject closestPlayer = null;
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Player");

        if (allEnemies.Length > 0)
        {
            foreach (GameObject currentPlayer in allEnemies)
            {
                float distanceToEnemy = (currentPlayer.transform.position - this.transform.position).sqrMagnitude;

                if (distanceToEnemy < distanceToClosestEnemy)
                {
                    distanceToClosestEnemy = distanceToEnemy;
                    closestPlayer = currentPlayer;
                }
            }
            playerPosition = closestPlayer.transform;
            //Debug.DrawLine(this.transform.position, closestPlayer.transform.position); // Show Player Detection
        }
        else
        {
            playerPosition = null;
        }
    }

    public Vector3 EnemyDirection()
    {
        Vector3 direction = playerPosition.transform.position - transform.position;
        return direction;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = detected ? Color.green : Color.red;

        if (stats.angle <= 0f) return;

        float halfVisionAngle = stats.angle * 0.5f;

        Vector3 p1, p2;

        p1 = PointForAngle(halfVisionAngle + 270, stats.distance);
        p2 = PointForAngle(-halfVisionAngle + 270, stats.distance);

        Gizmos.DrawLine(transform.position, transform.position + p1);
        Gizmos.DrawLine(transform.position, transform.position + p2);

        Gizmos.DrawRay(transform.position, -transform.up * stats.distance);
    }

    Vector3 PointForAngle(float angle, float distance)
    {
        return new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * distance;
    }
    #endregion

    public ObjectPooler GetObjectPooler()
    {
        return objectPooler;
    }

    public void AssignPath(int number)
    {
        waypoints = path.GetChild(number).transform;
        followPath = true;
        currentWaypoint = 0;
    }

    public void TransitionToState(State nextState)
    {
        if (currentState != nextState)
        {
            currentState.Exit(this);
            currentState = nextState;
            currentState.Enter(this);
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            collision.collider.GetComponent<IHealth>().Damage(stats.collissionDamage);
            Death();
        }
    }

    public void SetShootDecision()
    {
        switch (shootDecision)
        {
            case ShootDecision.None:
                break;

            case ShootDecision.Random:
                StartCoroutine(ShootRandom());
                break;

            case ShootDecision.Interval:
                Invoke("ShootOnInterval", Random.Range(0f, shootIntervalMaxWait));
                break;

            case ShootDecision.OnDetection:
                InvokeRepeating("ShootOnDetection", 0f, 0.2f);
                break;
        }
    }

    public void DisableShootDecision()
    {
        canShoot = false;
        switch (shootDecision)
        {
            case ShootDecision.None:
                break;

            case ShootDecision.Random:
                StopCoroutine(ShootRandom());
                break;

            case ShootDecision.Interval:
                CancelInvoke("ShootOnInterval");
                break;

            case ShootDecision.OnDetection:
                CancelInvoke("ShootOnDetection");
                break;
        }
    }

    IEnumerator ShootRandom()
    {
        while (true)
        {
            Debug.Log(canShoot);
            yield return new WaitForSeconds(Random.Range(minRandomShoot, maxRandomShoot));
            canShoot = true;
            yield return new WaitForSeconds(.1f);
            canShoot = false;
        }
    }

    private void ShootOnInterval()
    {
        canShoot = true;
    }


    private void ShootOnDetection()
    {
        DetectPlayer();
        canShoot = false;
        if (detected)
        {
            canShoot = true;
        }
    }
    
}
