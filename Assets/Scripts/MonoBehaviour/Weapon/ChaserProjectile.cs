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
        Vector3 moveDirection = enemyPosition.position - transform.position;

        if (moveDirection != Vector3.zero)
        {
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg - 90;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        transform.position = Vector3.MoveTowards(transform.position, enemyPosition.position, projectileSpeed * Time.deltaTime);
    }

    public void SetEnemyPosition(Transform enemy)
    {
        enemyPosition = enemy;
    }
}
