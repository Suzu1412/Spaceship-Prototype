using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [HideInInspector] public bool fired;
    [HideInInspector] public float projectileLifeTime;
    [HideInInspector] public int damage;
    [HideInInspector] public float projectileSpeed;
    [HideInInspector] public Vector3 moveDirection;
    [HideInInspector] public PlayerController playerWhoShot;
    [HideInInspector] public float bouncingRange;
    [HideInInspector] public float chanceToInstakill;
    [HideInInspector] public bool chaseTarget;
    [HideInInspector] public bool bounceTarget;
    [HideInInspector] public List<SpecialEffectSO> effectList;
    [HideInInspector] public GameObject closestEnemy;
    [HideInInspector] public GameObject currentEnemy;
    [HideInInspector] public int maxResistance = 0;
    private int _currentResistance;
    private ObjectPooler _objectPooler;

    void Awake()
    {
        _objectPooler = ObjectPooler.Instance;
    }

    private void OnEnable()
    {
        _currentResistance = maxResistance;
    }

    protected void OnDisable()
    {
        fired = false;
        chaseTarget = false;
        bounceTarget = false;
        currentEnemy = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (fired)
        {
            DisableProjectile();
            Movement();
        }
    }

    void DisableProjectile()
    {
        projectileLifeTime -= Time.deltaTime;
        if (projectileLifeTime <= 0)
        {
            this.gameObject.SetActive(false);
        }
    }

    void Movement()
    {
        if (chaseTarget && closestEnemy != null && closestEnemy.activeInHierarchy)
        {
            ChaseTarget();
        }
        else if (bounceTarget && closestEnemy != null && !closestEnemy.activeInHierarchy) //If projectile bounced and has no valid target then disable
        {
                gameObject.SetActive(false);
        }
        else
        {
            transform.Translate(Vector3.up * projectileSpeed * Time.deltaTime);
        }
    }

    public void SetMoveDirection(Vector3 direction)
    {
        moveDirection = direction;
        if (moveDirection != Vector3.zero)
        {
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg - 90;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<IHealth>() != null)
        {
            BeforeHitEffect(other);
            other.GetComponent<IHealth>().Damage(damage);

            //Only the Player can Set on Projectile
            if (playerWhoShot != null) other.GetComponent<EnemyController>().SetPlayer(playerWhoShot);

            _objectPooler.SpawnFromPool("Impact", transform.position, Quaternion.identity);
            AfterHitEffect(other);
            ReduceResistance();
        }
    }

    void OnBecameInvisible()
    {
        this.gameObject.SetActive(false);
    }

    #region Resistance
    public void AddResistance(int amount)
    {
        _currentResistance += amount;
    }

    private void ReduceResistance()
    {
        _currentResistance--;

        if (_currentResistance < 0)
        {
            this.gameObject.SetActive(false);
        }
    }
    #endregion

    #region Move Towards enemy
    public GameObject PointTowardsClosestEnemy()
    {
        GameObject enemyPosition = null;
        float distanceToClosestEnemy = Mathf.Infinity;
        closestEnemy = null;
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (allEnemies.Length > 0)
        {
            foreach (GameObject enemy in allEnemies)
            {
                if (enemy != currentEnemy)
                {
                    float distanceToEnemy = (enemy.transform.position - this.transform.position).sqrMagnitude;

                    if (distanceToEnemy < distanceToClosestEnemy)
                    {
                        distanceToClosestEnemy = distanceToEnemy;
                        closestEnemy = enemy;
                    }
                }
            }

            if (closestEnemy != null)
            {
                enemyPosition = closestEnemy;
            }
            else
            {
                enemyPosition = null;
            }

        }

        return enemyPosition;
    }

    private void ChaseTarget()
    {
        Vector3 moveDirection = closestEnemy.transform.position - transform.position;

        if (moveDirection != Vector3.zero)
        {
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg - 90;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        transform.position = Vector3.MoveTowards(transform.position, closestEnemy.transform.position, projectileSpeed * Time.deltaTime);
    }
    #endregion

    #region Special Effect
    public void InitializeEffect()
    {
        if (effectList.Count == 0) return;
        for (int i = 0; i < effectList.Count; i++)
        {
            effectList[i].Initialize(this);
        }
    }

    void BeforeHitEffect(Collider2D other)
    {
        if (effectList.Count != 0)
        {
            for (int i = 0; i < effectList.Count; i++)
            {
                effectList[i].BeforeHit(this, other.GetComponent<EnemyController>());
            }
        }
    }

    void AfterHitEffect(Collider2D other)
    {
        if (effectList.Count != 0)
        {
            for (int i = 0; i < effectList.Count; i++)
            {
                effectList[i].AfterHit(this, other.GetComponent<EnemyController>());
            }
        }
    }
    #endregion
}
