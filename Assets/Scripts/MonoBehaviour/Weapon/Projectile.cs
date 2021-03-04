using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public bool fired;
    public float projectileLifeTime;
    public int damage;
    public float projectileSpeed;
    public Vector3 moveDirection;
    [HideInInspector] public PlayerController playerWhoShot;

    private void OnDisable()
    {
        fired = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (fired)
        {
            DisableProjectile();
            Movement();
        }
    }

    private void DisableProjectile()
    {
        projectileLifeTime -= Time.deltaTime;
        if (projectileLifeTime <= 0)
        {
            this.gameObject.SetActive(false);
        }
    }

    private void Movement()
    {
        transform.Translate(moveDirection * projectileSpeed * Time.deltaTime);
    }

    public void SetMoveDirection(Vector3 direction)
    {
        moveDirection = direction;
    }

    private void OnTriggerEnter2D(Collider2D other)
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
            this.gameObject.SetActive(false);
        }
    }

    private void OnBecameInvisible()
    {
        this.gameObject.SetActive(false);
    }
}
