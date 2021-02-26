using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public bool fired;
    public float projectileLifeTime;
    public bool up;
    private bool flipped;
    public int damage;
    public float projectileSpeed;
    public Vector3 moveDirection;


    private void OnDisable()
    {
        fired = false;
    }

    // Update is called once per frame
    void Update()
    {
        DisableProjectile();
        Movement();
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
        /*
        if (fired)
        {
            if (up)
            {
                transform.Translate(Vector2.up * projectileSpeed * Time.deltaTime);

                if (flipped)
                {
                    flipped = false;
                    transform.localScale = new Vector2(transform.localScale.x, -transform.localScale.y);
                }
            }
            else
            {
                if (!flipped)
                {
                    flipped = true;
                    transform.localScale = new Vector2(transform.localScale.x, -transform.localScale.y);
                }

                transform.Translate(Vector2.down * projectileSpeed * Time.deltaTime);
         
            }
        }
        */
        
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
            this.gameObject.SetActive(false);
        }
    }
}
