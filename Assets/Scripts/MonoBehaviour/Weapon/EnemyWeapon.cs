using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : Weapon
{
    private EnemyController controller;

    protected override void Awake()
    {
        base.Awake();
        controller = GetComponent<EnemyController>();   
    }

    protected override void Update()
    {
        base.Update();
        if (controller.canShoot)
        {
            FireWeapon();
        }
    }

    protected override void FireWeapon()
    {
        for (int i = 0; i < weaponList.Count; i++)
        {
            weaponList[i].timeUntilNextShot -= Time.deltaTime;

            if (weaponList[i].timeUntilNextShot <= 0f)
            {
                for (int j = 0; j < weaponList[i].amountToShoot; j++)
                {
                    currentProjectile = objectPooler.SpawnFromPool(weaponList[i].weaponType.projectile.name, new Vector3(0f, 0f, 0f), Quaternion.identity);

                    if (currentProjectile != null)
                    {
                        ShootPattern(i, j);
                    }
                }

                weaponList[i].timeUntilNextShot = weaponList[i].timeBetweenShots + weaponList[i].weaponType.shootRate;
            }
        }
    }

    public override void SetProjectileValues(Transform gunPosition, float xPos, float yPos, float angle, int currentWeapon)
    {
        base.SetProjectileValues(gunPosition, xPos, yPos, -angle, currentWeapon);
        currentProjectile.GetComponent<Projectile>().damage = controller.stats.damage;
    }

    protected override void DirectionTowardsEnemyBullet(int i, int pointer)
    {
        Transform enemyPosition = PointTowardsClosestEnemy();
        if (enemyPosition != null)
        {
            Vector3 position = CalculatebulletPosition(i, pointer);

            Vector3 direction = enemyPosition.position - weaponList[i].gunPosition.position;
            direction.Normalize();
            float rotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            SetProjectileValues(weaponList[i].gunPosition, position.x, position.y, -rotation, i);
        }
        else
        {
            StraightBullet(i, pointer);
        }
    }

    protected override Transform PointTowardsClosestEnemy()
    {
        Transform enemyPosition = null;
        float distanceToClosestEnemy = Mathf.Infinity;
        GameObject closestEnemy = null;
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Player");

        if (allEnemies.Length > 0)
        {
            foreach (GameObject enemy in allEnemies)
            {
                float distanceToEnemy = (enemy.transform.position - this.transform.position).sqrMagnitude;

                if (distanceToEnemy < distanceToClosestEnemy)
                {
                    distanceToClosestEnemy = distanceToEnemy;
                    closestEnemy = enemy;
                }
            }
            enemyPosition = closestEnemy.transform;
            //Debug.DrawLine(this.transform.position, closestPlayer.transform.position); // Show Player Detection
        }

        return enemyPosition;
    }
}
