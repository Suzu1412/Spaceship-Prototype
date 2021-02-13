using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : Weapon
{
    private PlayerController player;

    protected void Start()
    {
        player = GetComponent<PlayerController>();

        GameObject newPool = new GameObject();
        projectileParentFolder = newPool;
        player.GetObjectPooler().CreatePool(mainWeapon, currentPool, projectileParentFolder);
    }

    protected override void Update()
    {
        base.Update();
        if (timeUntilNextShoot <= 0)
            FireWeapon();
    }

    protected override void FireWeapon()
    {
        for (int i = 0; i < mainWeapon.pattern.bulletAmount[player.currentLevel]; i++)
        {
            currentProjectile = player.GetObjectPooler().GetObject(currentPool);

            if (currentProjectile != null)
            {
                mainWeapon.pattern.PlaceProjectile(this, player.currentLevel, i);
            }
        }

        ResetShoot(player.stats.shootRate);
    }

    public override void SetProjectileValues(float offset)
    {
        base.SetProjectileValues(offset);
        currentProjectile.GetComponent<Projectile>().up = true;
        currentProjectile.GetComponent<Projectile>().projectileLifeTime = player.stats.projectileDuration;
        currentProjectile.GetComponent<Projectile>().projectileSpeed = player.stats.projectileSpeed;
        currentProjectile.GetComponent<Projectile>().damage = player.stats.damage;
        currentProjectile.SetActive(true);
    }
        
}
