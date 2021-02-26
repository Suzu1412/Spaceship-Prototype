using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] protected WeaponType mainWeapon;
    protected GameObject projectileParentFolder;
    public Transform gunPosition;
    [SerializeField] protected Transform gunRotation;
    public GameObject currentProjectile { get; protected set; }

    protected ObjectPooler objectPooler;
    protected float timeUntilNextShoot;
    private Vector2 bulletDirection;

    protected virtual void Awake()
    {
        objectPooler = ObjectPooler.Instance;

        ObjectPooler.Pool item = new ObjectPooler.Pool
        {
            prefab = mainWeapon.projectile,
            shouldExpandPool = true,
            size = mainWeapon.amountToPool,
            tag = mainWeapon.projectile.name,
            isChild = true
        };

        objectPooler.AddPool(item);
    }

    protected virtual void Update()
    {
        timeUntilNextShoot -= Time.deltaTime;
    }

    protected abstract void FireWeapon();

    public virtual void SetProjectileValues(float xPos, float yPos, float angle)
    {
        currentProjectile.transform.position = new Vector3(gunPosition.position.x + xPos, gunPosition.position.y + yPos, 0);
        currentProjectile.transform.rotation = gunPosition.rotation;

        float bulDirX = gunPosition.position.x + Mathf.Cos((angle * Mathf.PI) / 180f);
        float bulDirY = gunPosition.position.y + Mathf.Sin((angle * Mathf.PI) / 180f);

        Vector3 bulMoveVector = new Vector3(bulDirX, bulDirY, 0f);
        bulletDirection = (bulMoveVector - transform.position).normalized;

        currentProjectile.GetComponent<Projectile>().fired = true;
        currentProjectile.GetComponent<Projectile>().projectileLifeTime = mainWeapon.lifeTime;
        currentProjectile.GetComponent<Projectile>().projectileSpeed = mainWeapon.projectileSpeed;
        currentProjectile.GetComponent<Projectile>().SetMoveDirection(bulletDirection);
        currentProjectile.SetActive(true);
    }

    public void ResetShoot(float delay)
    {
        timeUntilNextShoot = delay;
    }

    public ObjectPooler GetObjectPooler()
    {
        return objectPooler;
    }
}
