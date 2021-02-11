using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] protected WeaponType weapon;

    public bool fired;
    public float projectileLifeTime;
    public bool up;
    private bool flipped;


    void OnEnable()
    {
        projectileLifeTime = weapon.lifeTime;
    }

    private void OnDisable()
    {
        fired = false;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    public void Movement()
    {
        if (fired)
        {
            projectileLifeTime -= Time.deltaTime;

            if (projectileLifeTime > 0)
            {
                if (up)
                {
                    transform.Translate(Vector2.up * weapon.projectileSpeed * Time.deltaTime);

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

                    transform.Translate(Vector2.down * weapon.projectileSpeed * Time.deltaTime);
                }
            }
            else
            {
                this.gameObject.SetActive(false);
            }
        }
    }
}
