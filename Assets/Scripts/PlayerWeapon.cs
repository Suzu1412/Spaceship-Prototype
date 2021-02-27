using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : Weapon
{
    private PlayerController controller;

    protected override void Awake()
    {
        base.Awake();
        controller = GetComponent<PlayerController>();

        for(int i=0; i < weaponList.Count; i++)
        {
            ObjectPooler.Pool item = new ObjectPooler.Pool
            {
                prefab = weaponList[i].weaponType.projectile,
                shouldExpandPool = true,
                size = weaponList[i].weaponType.amountToPool,
                tag = weaponList[i].weaponType.projectile.name,
                isChild = true
            };
            objectPooler.AddPool(item);
        }
    }

    protected override void Update()
    {
        base.Update();
        if (controller.canShoot)
            FireWeapon();
    }

    protected override void FireWeapon()
    {
        weaponList[controller.currentLevel].timeUntilNextShot -= Time.deltaTime;

        if (weaponList[controller.currentLevel].timeUntilNextShot <= 0f)
        {
            weaponList[controller.currentLevel].timeUntilNextShot = weaponList[controller.currentLevel].timeBetweenShots + weaponList[controller.currentLevel].weaponType.shootRate;

            for (int i = 0; i < weaponList[controller.currentLevel].amountToShoot; i++)
            {
                currentProjectile = objectPooler.SpawnFromPool(weaponList[controller.currentLevel].weaponType.projectile.name, new Vector3(0f, 0f, 0f), Quaternion.identity);

                if (currentProjectile != null)
                {
                    ShootPattern(controller.currentLevel, i);
                }
            }
        }
    }

    public override void SetProjectileValues(Transform gunPosition, float xPos, float yPos, float angle)
    {
        
        base.SetProjectileValues(gunPosition, xPos, yPos, angle);
        currentProjectile.GetComponent<Projectile>().damage = controller.stats.damage;
    }
        

    protected override Transform PointTowardsClosestEnemy()
    {
        Transform enemyPosition = null;
        float distanceToClosestEnemy = Mathf.Infinity;
        GameObject closestEnemy = null;
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
            //Debug.DrawLine(this.transform.position, closestPlayer.transform.position); // Show Player Detection
        }

        return enemyPosition;
    }
}
