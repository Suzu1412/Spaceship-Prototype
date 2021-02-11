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

    public List<GameObject> currentPool = new List<GameObject>();
    protected float timeUntilNextShoot;

    protected virtual void Update()
    {
        timeUntilNextShoot -= Time.deltaTime;
    }

    protected abstract void FireWeapon();

    public virtual void SetProjectileValues(float offset)
    {
        currentProjectile.transform.position = gunPosition.position + new Vector3(offset, 0, 0);

        currentProjectile.SetActive(true);
        currentProjectile.GetComponent<Projectile>().fired = true;

    }

    public void ResetShoot()
    {
        timeUntilNextShoot = mainWeapon.shootRate;
    }
}
