using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ProjectilePattern/Circular", order = 2)]
public class CircularPattern : Pattern
{
    [Range(0f, 360f)] public float startAngle, endAngle;

    public override void PlaceProjectile(Weapon weapon, int level, int pointer)
    {
        float angleStep = (endAngle - startAngle) / (bulletAmount[level] -1);
        float angle = startAngle + (angleStep * pointer);

        float bulDirX = Mathf.Sin((angle * Mathf.PI)) / 180f;
        float bulDirY = Mathf.Cos((angle * Mathf.PI)) / 180f;

        Vector3 bulMoveVector = new Vector3(bulDirX, bulDirY, 0f);
        

        weapon.SetProjectileValues(bulDirX, bulDirY, angle);
    }
}
