using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenetratingProjectile : Projectile
{
    [SerializeField] private int maxProjectileResistance;
    private int projectileResistance;

    private void OnEnable()
    {
        projectileResistance = maxProjectileResistance;
    }


    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<IHealth>() != null)
        {
            other.GetComponent<IHealth>().Damage(damage);

            if (playerWhoShot != null) //Only the Player can Set on Projectile
            {
                if (other.GetComponent<EnemyController>() != null)
                {
                    other.GetComponent<EnemyController>().SetPlayer(playerWhoShot);
                }
            }

            _objectPooler.SpawnFromPool("Impact", transform.position, Quaternion.identity);

            projectileResistance -= 1;

            if (projectileResistance <= 0)
            {
                this.gameObject.SetActive(false);
            }
        }
    }
}
