using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapons", fileName = "WeaponType", order = 1)]
public class WeaponType : ScriptableObject
{
    public GameObject projectile;
    public float projectileSpeed;
    public int amountToPool;

    public float shootRate;
    public float lifeTime;

    public Pattern pattern;
}
