using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Special Effect/Offensive/Instakill Target")]
public class InstakillEffect : SpecialEffectSO
{
    public override void BeforeHit(Projectile projectile, CharController enemy)
    {
        if (enemy != null && enemy.maxHealth < 150)
        {
            float randomChance = Random.Range(0f, 1f);

            if (randomChance < projectile.chanceToInstakill)
            {
                enemy.Death();
                return;
            }
        }

        if (enemy != null && enemy.maxHealth >= 150)
        {
            if ((float)enemy.currentHealth / enemy.maxHealth < 0.5f)
            {
                float randomChance = Random.Range(0f, 1f);

                if (randomChance < projectile.chanceToInstakill)
                {
                    enemy.Death();
                    return;
                }
            }
        }

        return;
    }
}
