using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : Weapon
{
    private EnemyController controller;

    protected void Start()
    {
        controller = GetComponent<EnemyController>();

        GameObject newPool = new GameObject();
        projectileParentFolder = newPool;
        controller.GetObjectPooler().CreatePool(mainWeapon, currentPool, projectileParentFolder);
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
            currentProjectile = controller.GetObjectPooler().GetObject(currentPool);

            if (currentProjectile != null)
            {
                mainWeapon.pattern.PlaceProjectile(this, 0, i);
            }
        }

        ResetShoot(controller.stats.shootRate);
    }

    public override void SetProjectileValues(float offset)
    {
        base.SetProjectileValues(offset);
        currentProjectile.GetComponent<Projectile>().up = false;
        currentProjectile.GetComponent<Projectile>().projectileLifeTime = controller.stats.projectileDuration;
        currentProjectile.GetComponent<Projectile>().projectileSpeed = controller.stats.projectileSpeed;
        currentProjectile.GetComponent<Projectile>().damage = controller.stats.damage;
        currentProjectile.SetActive(true);
    }

}
