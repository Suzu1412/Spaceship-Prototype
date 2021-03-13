using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Special Effect/Offensive/Thrust")]
public class ThrustEffect : SpecialEffectSO
{
    /// <summary>
    /// Add maximum projectile resistance. Keeps moving in same direction after hitting the enemy so no more code needed.
    /// </summary>
    public override void Initialize(Projectile projectile)
    {
        projectile.AddResistance(1);
    }
}
