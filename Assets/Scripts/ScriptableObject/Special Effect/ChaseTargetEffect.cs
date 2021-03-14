using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Special Effect/Offensive/Chase Target")]
public class ChaseTargetEffect : SpecialEffectSO
{
    /// <summary>
    /// ChaseTargetEffect: Is applied on the initialize of the bullet to start moving towards target. Chase any target on screen
    /// BounceTargetEffect: Is applied AFTER hitting a target to choose a new Target. Chase only if target is in homing range.
    /// </summary>
    public override void Initialize(Projectile projectile)
    {
        projectile.PointTowardsClosestEnemy();
        projectile.chaseTarget = true;
    }
}
