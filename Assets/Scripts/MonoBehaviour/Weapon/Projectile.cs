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
    protected ObjectPooler _objectPooler;

    protected void OnDisable()
    {
        fired = false;
    }

    protected void Awake()
    {
        _objectPooler = ObjectPooler.Instance;
    }

    // Update is called once per frame
    protected void Update()
    {
        if (fired)
        {
            DisableProjectile();
            Movement();
        }
    }

    protected void DisableProjectile()
    {
        projectileLifeTime -= Time.deltaTime;
        if (projectileLifeTime <= 0)
        {
            this.gameObject.SetActive(false);
        }
    }

    protected virtual void Movement()
    {
        transform.Translate(moveDirection * projectileSpeed * Time.deltaTime);
    }

    public void SetMoveDirection(Vector3 direction)
    {
        moveDirection = direction;
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
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
            
            this.gameObject.SetActive(false);
        }
    }

    protected void OnBecameInvisible()
    {
        this.gameObject.SetActive(false);
    }
}
