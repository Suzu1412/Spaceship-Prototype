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
        if (controller.canShoot && timeUntilNextShoot <= 0)
        {
            FireWeapon();
        }
    }

    protected override void FireWeapon()
    {
        for (int i = 0; i < mainWeapon.pattern.bulletAmount[0]; i++)
        {
            currentProjectile = objectPooler.SpawnFromPool(mainWeapon.projectile.name, new Vector3(0f, 0f, 0f), Quaternion.identity);

            if (currentProjectile != null)
            {
                mainWeapon.pattern.PlaceProjectile(this, 0, i);
            }
        }

        ResetShoot(mainWeapon.shootRate);
    }

    public override void SetProjectileValues(float offset)
    {
        base.SetProjectileValues(offset);
        currentProjectile.GetComponent<Projectile>().up = false;
        currentProjectile.GetComponent<Projectile>().projectileLifeTime = mainWeapon.lifeTime;
        currentProjectile.GetComponent<Projectile>().projectileSpeed = mainWeapon.projectileSpeed;
        currentProjectile.GetComponent<Projectile>().damage = controller.stats.damage;
        currentProjectile.SetActive(true);
    }

}
