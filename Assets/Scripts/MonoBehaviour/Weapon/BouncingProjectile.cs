using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingProjectile : Projectile
{
    public int bounceMax;
    public float maxBounceDistance;
    private int bounceLeft;
    [SerializeField] private Transform enemyPosition;
    private GameObject closestEnemy;
    private GameObject currentEnemy;
    private bool bounce;

    private void OnEnable()
    {
        enemyPosition = null;
        currentEnemy = null;
        bounceLeft = bounceMax;
        bounce = false;
    }

    protected override void Movement()
    {
        if (enemyPosition != null && enemyPosition.gameObject.activeInHierarchy)
        {
            ChaseTarget();
        }
        else
        {
            base.Movement();
        }
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<IHealth>() != null)
        {
            other.GetComponent<IHealth>().Damage(damage);

            if (playerWhoShot != null) //Only the Player can Set on Projectile
            {
                if (other.GetComponent<EnemyController>() != null)
                {
                    other.GetComponent<EnemyController>().SetPlayer(playerWhoShot);
                }
            }

            _objectPooler.SpawnFromPool("Impact", transform.position, Quaternion.identity);

            bounceLeft -= 1;

            if (bounceLeft < 0)
            {
                this.gameObject.SetActive(false);
            }
            else
            {
                currentEnemy = other.gameObject;
                Bounce();
            }
        }
    }

    private void Bounce()
    {
        enemyPosition = PointTowardsClosestEnemy();

        if (enemyPosition == null)
        {
            this.gameObject.SetActive(false);
        }

        if (!bounce)
        {
            bounce = true;
            damage = (int)Mathf.Round(damage / 2);
            projectileSpeed = projectileSpeed / 2;
        }
    }

    private Transform PointTowardsClosestEnemy()
    {
        Transform enemyPosition = null;
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

                    if (distanceToEnemy < distanceToClosestEnemy && distanceToEnemy < maxBounceDistance)
                    {
                        distanceToClosestEnemy = distanceToEnemy;
                        closestEnemy = enemy;
                    }
                }
            }
            
            if (closestEnemy != null)
            {
                enemyPosition = closestEnemy.transform;
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
        transform.position = Vector3.MoveTowards(transform.position, enemyPosition.position, projectileSpeed * Time.deltaTime);
    }
}
