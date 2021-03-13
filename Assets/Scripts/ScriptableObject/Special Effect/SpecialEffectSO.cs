using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpecialEffectSO : ScriptableObject
{
    public bool permanent;
    public float duration;
    public bool offensive;

    /// <summary>
    /// Set the initial values of the projectile, such as resistance, added damage, direction, enemy position, etc.
    /// </summary>
    public virtual void Initialize(Projectile projectile)
    {

    }

    /// <summary>
    /// Apply this before the projectile deals damage. Used for Instakill Effect (Doesn't need to deal damage)
    /// Since it won't be used frequently its made virtual so it doesn't need to be overriden in each class
    /// </summary>
    public virtual void BeforeHit(Projectile projectile, CharController enemy)
    {

    }

    /// <summary>
    /// Apply this after the damage has been made, like applying elemental effect or to bounce to another target
    /// </summary>
    public virtual void AfterHit(Projectile projectile, CharController enemy)
    {
    }
}
