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
    [HideInInspector] public int maxResistance = 0;
    [HideInInspector] public float homingRange;
    [HideInInspector] public float chanceToInstakill;
    public bool chaseTarget;
    public bool bounceTarget;
    protected ObjectPooler _objectPooler;
    public List<SpecialEffectSO> effectList;
    public GameObject closestEnemy;
    public GameObject currentEnemy;
    private int _currentResistance;


    protected void OnDisable()
    {
        fired = false;
        chaseTarget = false;
        bounceTarget = false;
        currentEnemy = null;
    }

    private void OnEnable()
    {
        _currentResistance = maxResistance;
    }

    public void ActivateEffect()
    {
        if (effectList.Count == 0) return;
        for (int i=0; i < effectList.Count; i++)
        {
            effectList[i].Initialize(this);
        }
    }

    protected void Awake()
    {
        _objectPooler = ObjectPooler.Instance;
    }

    // Update is called once per frame
    protected void Update()
    {
        if (fired)
        {
            DisableProjectile();
            Movement();
        }
    }

    protected void DisableProjectile()
    {
        projectileLifeTime -= Time.deltaTime;
        if (projectileLifeTime <= 0)
        {
            this.gameObject.SetActive(false);
        }
    }

    protected virtual void Movement()
    {
        if (chaseTarget && closestEnemy != null && closestEnemy.activeInHierarchy)
        {
            ChaseTarget();
        }
        else if (bounceTarget && closestEnemy != null && !closestEnemy.activeInHierarchy)
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

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<IHealth>() != null)
        {
            if (effectList.Count != 0)
            {
                for (int i = 0; i < effectList.Count; i++)
                {
                    effectList[i].BeforeHit(this, other.GetComponent<EnemyController>());
                }
            }

            other.GetComponent<IHealth>().Damage(damage);

            if (playerWhoShot != null) //Only the Player can Set on Projectile
            {
                if (other.GetComponent<EnemyController>() != null)
                {
                    other.GetComponent<EnemyController>().SetPlayer(playerWhoShot);
                }
            }

            _objectPooler.SpawnFromPool("Impact", transform.position, Quaternion.identity);

            if (effectList.Count != 0)
            {
                for (int i = 0; i < effectList.Count; i++)
                {
                    effectList[i].AfterHit(this, other.GetComponent<EnemyController>());
                }
            }

            ReduceResistance();
        }
    }

    protected void OnBecameInvisible()
    {
        this.gameObject.SetActive(false);
    }

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
}
