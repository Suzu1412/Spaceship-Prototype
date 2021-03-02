using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShootDecision
{
    None, 
    Interval,
    OnDetection,
    RandomSeconds
    //Shoot On Waypoint
}

public enum PathToTake
{
    None = -1,
    Fighter = 0,
    Rogue = 1,
    Ranger = 2
}

public class EnemyController : CharController
{
    #region Variables
    [Header("Enemy Attributes")]
    private State _initialState;
    [SerializeField] private EnemyStats _stats;
    [SerializeField] private State _currentState;

    [Header("Decision Making")]
    private bool _canDetect = true;
    private bool _detected = true;
    private bool _canShoot = true;
    private Transform _playerPosition;
    private float _waitForSeconds = 0f;

    [Header("Follow Path Attributes")]
    private Transform _path;
    private Transform _waypoints;
    //SerializeField private List<int> _waypointToShoot = new List<int>(); //If on Waypoint shoot, else don't
    private int _currentWaypoint;
    private bool _followPath;
    [SerializeField] private PathToTake _pathToTake;
    
    [Header("Shoot")]
    public ShootDecision shootDecision;
    [Tooltip("On Random Shoot Decision")] [Range(0f, 3f)] [SerializeField] private float _minRandomShoot = 1f;
    [Tooltip("On Random Shoot Decision")] [Range(0f, 5f)] [SerializeField] private float _maxRandomShoot = 5f;
    [Tooltip("On Interval Shoot Decision")] [Range(0f, 2f)] [SerializeField] private float _shootIntervalMaxWait = 1f;
    #endregion

    #region Properties
    public EnemyStats stats { get { return _stats; } }
    public Transform playerPosition { get { return _playerPosition; } }
    public bool detected { get { return _detected; } }
    public bool canShoot { get { return _canShoot; } }
    public int currentWayPoint { get { return _currentWaypoint; } }
    public bool followPath { get { return _followPath; } }
    public Transform waypoints { get { return _waypoints; } }
    public float minRandomShoot { get { return _minRandomShoot; } }
    public float maxRandomShoot { get { return _maxRandomShoot; } }
    public float shootIntervalMaxWait { get { return _shootIntervalMaxWait; } }
    #endregion


    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        _path = GameObject.Find("Path").transform;
        _initialState = _currentState;
        if (_initialState == null) Debug.Log(this.gameObject.name + " missing CurrentState");
        if (_stats == null) Debug.Log(this.gameObject.name + " missing Stats");
    }

    void OnEnable()
    {
        _currentHealth = stats.maxHealth;
        isDeath = false;
        _currentState = _initialState;
        _currentState.Enter(this);
        _manager.AddEnemyCount();
    }

    void OnDisable()
    {
        _currentState.Exit(this);
        _manager.RemoveEnemyCount();
    }

    // Update is called once per frame
    void Update()
    {
        _currentState.LogicUpdate(this);
    }
    
    void FixedUpdate()
    {
        _currentState.PhysicsUpdate(this);

        if (transform.position.y < -6 || transform.position.y > 9) 
        {
            this.gameObject.SetActive(false);
        }
    }

    #region IHealth Implementation
    public override void Heal(int amount)
    {

        if (_currentHealth + amount >= stats.maxHealth)
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
        //impactEffect.SpawnFromPool(this.name, this.transform.position, Quaternion.identity);
        //GameObject explosion = explosionEffect.SpawnFromPool(this.tag, this.transform.position, Quaternion.identity);
        //explosion.GetComponent<ParticleSystem>().Play();

        if (!isDeath)
        {
            isDeath = true;
            if (score != null) score.SetScore(stats.score);
            this.gameObject.SetActive(false);
        }
        
    }
    #endregion

    #region Player Detection
    public void DetectPlayer()
    {

        _detected = false;

        FindClosestPlayer();

        if (_canDetect)
        {
            if (playerPosition != null)
            {
                Vector2 PlayerVector = playerPosition.position - transform.position;

                if (Vector3.Angle(PlayerVector.normalized, -transform.up) < stats.angle * 0.5f)
                {
                    if (PlayerVector.magnitude < stats.distance)
                    {
                        _detected = true;
                    }
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
            _playerPosition = closestPlayer.transform;
            //Debug.DrawLine(this.transform.position, closestPlayer.transform.position); // Show Player Detection
        }
        else
        {
            _playerPosition = null;
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

        if (stats == null) return;

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

    #region Path
    public void AssignPath(int number)
    {
        if (_waypoints == null)
        {
            _waypoints = _path.GetChild((int)_pathToTake).GetChild(number).transform;
        }
        _followPath = true;
        _currentWaypoint = 0;
        this.transform.position = _waypoints.GetChild(_currentWaypoint).transform.position;
    }

    public void UpdatePath()
    {
        _currentWaypoint++;
        
        if (currentWayPoint >= waypoints.childCount)
        {
            _currentWaypoint = 0;
            _followPath = false;
        }
    }

    public void SetWaypoint(Transform waypoint)
    {
        _waypoints = waypoint;
        this.transform.position = _waypoints.GetChild(0).transform.position;
    }
    #endregion

    #region Shoot
    public void SetShootDecision()
    {
        switch (shootDecision)
        {
            case ShootDecision.None:
                _canShoot = false;
                break;

            case ShootDecision.Interval:
                Invoke("ShootOnInterval", shootIntervalMaxWait);
                break;

            case ShootDecision.OnDetection:
                InvokeRepeating("ShootOnDetection", 0f, 0.2f);
                break;

            case ShootDecision.RandomSeconds:

                break;
        }
    }

    public void DisableShootDecision()
    {
        _canShoot = false;
        switch (shootDecision)
        {
            case ShootDecision.None:
                break;

            case ShootDecision.Interval:
                CancelInvoke("ShootOnInterval");
                break;

            case ShootDecision.OnDetection:
                CancelInvoke("ShootOnDetection");
                break;

            case ShootDecision.RandomSeconds:
                //CancelInvoke("ShootRandom");
                break;
        }
    }

    private void ShootOnInterval()
    {
        _canShoot = true;
    }


    private void ShootOnDetection()
    {
        DetectPlayer();
        _canShoot = false;
        if (detected)
        {
            _canShoot = true;
        }
    }
    #endregion

    public bool WaitForSeconds(float seconds)
    {
        bool timeOver = false;

        if (_waitForSeconds <= 0f)
        {
            _waitForSeconds = seconds;
        }
        _waitForSeconds -= Time.deltaTime;

        if (_waitForSeconds <= 0f)
        {
            timeOver = true;
        }

        return timeOver;
    }

    public void TransitionToState(State nextState)
    {
        if (_currentState != nextState)
        {
            _currentState.Exit(this);
            _currentState = nextState;
            _currentState.Enter(this);
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

    private void OnBecameVisible()
    {
        _canDetect = true;
        _canShoot = true;
    }

    private void OnBecameInvisible()
    {
        _canDetect = false;
        _canShoot = false;
    }
}
