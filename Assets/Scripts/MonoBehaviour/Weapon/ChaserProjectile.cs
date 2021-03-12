using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaserProjectile : Projectile
{
    [SerializeField] private Transform enemyPosition;

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

    private void ChaseTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, enemyPosition.position, projectileSpeed * Time.deltaTime);
    }

    public void SetEnemyPosition(Transform enemy)
    {
        enemyPosition = enemy;
    }
}
