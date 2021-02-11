using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour, IHealth
{
    protected int _currentHealth;
    //[SerializeField] public EnemyStatsSO stats;
    [SerializeField] public Pattern pattern;
    protected Rigidbody2D rb;
    [HideInInspector] public Transform playerPosition;
    [HideInInspector] public float direction;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //_currentHealth = stats.maxHealth;
        direction = -1f;
        //pattern.Enter(this);
    }



    // Update is called once per frame
    void Update()
    {
        //pattern.Behaviour(this);
    }

    /*
    #region IHealth Implementation
    public void Heal(int amount)
    {

        if (_currentHealth + amount >= stats.maxHealth)
        {
            _currentHealth = stats.maxHealth;
        }
        else
        {
            _currentHealth += amount;
        }
    }

    public void Damage(int amount)
    {
        if (_currentHealth - amount <= 0)
        {
            Death();
        }
        else
        {
            _currentHealth -= amount;
        }
    }

    public void Death()
    {
        //impactEffect.SpawnFromPool(this.name, this.transform.position, Quaternion.identity);
        //GameObject explosion = explosionEffect.SpawnFromPool(this.tag, this.transform.position, Quaternion.identity);
        //explosion.GetComponent<ParticleSystem>().Play();

        this.gameObject.SetActive(false);
    }
    #endregion

    //public abstract void Move();



    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            collision.collider.GetComponent<IHealth>().Damage(stats.collisionDamage);
            Damage(999);
        }
    }
    */
}
