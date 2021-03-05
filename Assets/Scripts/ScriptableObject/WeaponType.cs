using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapons", fileName = "WeaponType", order = 1)]
public class WeaponType : ScriptableObject
{
    public GameObject projectile;
    public int amountToPool = 30;
    public float shootRate = 0.2f;
    public float lifeTime = 2f;

    [Header("Straight Bullet Variables")]
    public float initialOffsetX = 0.15f;
    public float offsetXBetweenShots = 0.3f;
    public float initialOffsetY = 0f;
    public float offsetYBetweenShots = 0f;

    [Header("Radius Bullet Variables")]
    [Range(0f, 360f)] public float startAngle = 0f;
    [Range(0f, 360f)] public float endAngle = 360f;

    [Header("Spiral Bullet Variables")]
    public float increaseAngle = 10f;
}

