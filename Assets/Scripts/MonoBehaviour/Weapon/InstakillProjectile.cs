using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstakillProjectile : Projectile
{
    [Range(0f, 1f)]public float chanceToInstakill = 0f;

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<IHealth>() != null)
        {
            bool enemyKilled = InstakillChance(other.GetComponent<CharController>());

            if (!enemyKilled)
            {
                other.GetComponent<IHealth>().Damage(damage);
            }
           
            if (playerWhoShot != null) //Only the Player can Set on Projectile
            {
                if (other.GetComponent<EnemyController>() != null)
                {
                    other.GetComponent<EnemyController>().SetPlayer(playerWhoShot);
                }
            }

            _objectPooler.SpawnFromPool("Impact", transform.position, Quaternion.identity);

            this.gameObject.SetActive(false);
        }
    }

    private bool InstakillChance(CharController enemy)
    {
        if (enemy != null && enemy.maxHealth < 150)
        {
            float randomChance = Random.Range(0f, 1f);

            if (randomChance < chanceToInstakill)
            {
                enemy.Death();
                return true;
            }
        }

        if (enemy != null && enemy.maxHealth >= 150)
        {
            if ((float)enemy.currentHealth / enemy.maxHealth < 0.5f)
            {
                float randomChance = Random.Range(0f, 1f);

                if (randomChance < chanceToInstakill)
                {
                    enemy.Death();
                    return true;
                }
            }
        }

        return false;
    }
}
