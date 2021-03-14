using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : Weapon
{
    private PlayerController controller;
    private GameObject closestEnemy;

    protected override void Awake()
    {
        base.Awake();
        controller = GetComponent<PlayerController>();
    }

    protected override void Update()
    {
        base.Update();
        if (controller.canShoot)
            FireWeapon();
    }

    protected override void FireWeapon()
    {
        weaponList[controller.stats.Level].timeUntilNextShot -= Time.deltaTime;

        if (weaponList[controller.stats.Level].timeUntilNextShot <= 0f)
        {
            weaponList[controller.stats.Level].timeUntilNextShot = weaponList[controller.stats.Level].timeBetweenShots + weaponList[controller.stats.Level].weaponType.shootRate;

            for (int i = 0; i < weaponList[controller.stats.Level].amountToShoot; i++)
            {
                currentProjectile = objectPooler.SpawnFromPool(weaponList[controller.stats.Level].weaponType.projectile.name, new Vector3(0f, 0f, 0f), Quaternion.identity).GetComponent<Projectile>();

                if (currentProjectile != null)
                {
                    ShootPattern(controller.stats.Level, i);
                }
            }
        }
    }

    public override void SetProjectileValues(Transform gunPosition, float xPos, float yPos, float angle, int currentWeapon)
    {
        
        base.SetProjectileValues(gunPosition, xPos, yPos, angle, currentWeapon);
        currentProjectile.damage = controller.stats.damage + weaponList[currentWeapon].addDamage;
        currentProjectile.AddResistance(weaponList[currentWeapon].addResistance);
        currentProjectile.playerWhoShot = controller;
        currentProjectile.effectList = controller.effectList;
        currentProjectile.homingRange = controller.stats.homingRange;
        currentProjectile.chanceToInstakill = controller.stats.chanceToInstakill;
        currentProjectile.ActivateEffect();
    }
        

    protected override Transform PointTowardsClosestEnemy()
    {
        Transform enemyPosition = null;
        float distanceToClosestEnemy = Mathf.Infinity;
        closestEnemy = null;
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");

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
        }

        return enemyPosition;
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

            SetProjectileValues(weaponList[i].gunPosition, position.x, position.y, rotation, i);
        }
        else
        {
            StraightBullet(i, pointer);
        }
    }
}
