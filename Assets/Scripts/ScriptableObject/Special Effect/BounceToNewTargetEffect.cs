using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Special Effect/Offensive/Bounce Target")]
public class BounceToNewTargetEffect : SpecialEffectSO
{
    /// <summary>
    /// ChaseTargetEffect: Is applied on the initialize of the bullet to start moving towards target. Chase any target on screen
    /// BounceTargetEffect: Is applied AFTER hitting a target to choose a new Target. Chase only if target is in homing range.
    /// </summary>
    public override void AfterHit(Projectile projectile, CharController enemy)
    {
        projectile.AddResistance(1);
        projectile.currentEnemy = enemy.gameObject;
        GameObject enemyPosition = projectile.PointTowardsClosestEnemy();
        projectile.chaseTarget = true;
        projectile.bounceTarget = true;

        if (enemyPosition == null || !enemyPosition.activeInHierarchy)
        {
            projectile.gameObject.SetActive(false);
            return;
        }

        if (Vector3.Distance(enemyPosition.transform.position, projectile.transform.position) > projectile.bouncingRange)
        {
            projectile.gameObject.SetActive(false);
        }
    }
}
