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
    public BulletPattern pattern;

    [Header("Straight Bullet Variables")]
    public float initialOffsetX = 0.15f;
    public float offsetXBetweenShots = 0.3f;

    [Header("Radius Bullet Variables")]
    [Range(0f, 360f)] public float startAngle = 0f;
    [Range(0f, 360f)] public float endAngle = 360f;

    [Header("Spiral Bullet Variables")]
    public float increaseAngle;
}

public enum BulletPattern
{
    Straight,
    Radius,
    Spiral,
    Random,
    DirectionToPlayer
}