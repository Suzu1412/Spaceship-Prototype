using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ProjectilePattern/Straight", order = 2)]
public class StraightBullets : Pattern
{
    public override void PlaceProjectile(Weapon weapon, int level, int pointer)
    {
        if (bulletAmount[level] == 1)
        {
            weapon.SetProjectileValues(0);
        }

        else if (bulletAmount[level] > 1)
        {
            if (bulletAmount[level] % 2 == 0)
            {
                if (pointer < bulletAmount[level] / 2)
                {
                    weapon.SetProjectileValues(initialOffset - offsetBetweenShots * (bulletAmount[level] / 2 - pointer));
                }
                else
                {
                    weapon.SetProjectileValues(-initialOffset + offsetBetweenShots * (pointer + 1 - bulletAmount[level] / 2));
                }
            }
            else
            {
                int centerBullet = Mathf.RoundToInt(bulletAmount[level] / 2);

                if (pointer < bulletAmount[level] / 2)
                {
                    weapon.SetProjectileValues(-offsetBetweenShots * (bulletAmount[level] / 2 - pointer));
                }
                else if (pointer == centerBullet)
                {
                    weapon.SetProjectileValues(0);
                }
                else
                {
                    weapon.SetProjectileValues(offsetBetweenShots * (pointer - bulletAmount[level] / 2));
                }
            }
        }
        
    }
}
