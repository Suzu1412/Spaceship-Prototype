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

    public virtual void SetProjectileValues(float offset)
    {
        currentProjectile.transform.position = gunPosition.position + new Vector3(offset, 0, 0);
        currentProjectile.GetComponent<Projectile>().fired = true;
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
