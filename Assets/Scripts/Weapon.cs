using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] protected List<WeaponList> weaponList;
    [SerializeField] protected WeaponType mainWeapon;
    //public Transform gunPosition;
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

    protected void OnEnable()
    {
        for (int i=0; i < weaponList.Count; i++)
        {
            weaponList[i].increaseAngle = weaponList[i].weaponType.increaseAngle;
        }
    }

    protected virtual void Update()
    {
        timeUntilNextShoot -= Time.deltaTime;
    }

    protected abstract void FireWeapon();

    public virtual void SetProjectileValues(Transform gunPosition, float xPos, float yPos, float angle)
    {
        currentProjectile.transform.position = new Vector3(gunPosition.position.x + xPos, gunPosition.position.y + yPos, 0);

        //C�digo original: �ngulo 0 = Vector2.Up;
        //float bulDirX = currentProjectile.transform.position.x + Mathf.Sin((angle * Mathf.PI) / 180f);
        //float bulDirY = currentProjectile.transform.position.y + Mathf.Cos((angle * Mathf.PI) / 180f);

        //C�digo modificado: �nglo 90 = Vector2.Up
        float bulDirX = currentProjectile.transform.position.x + Mathf.Cos((angle * Mathf.PI) / 180f);
        float bulDirY = currentProjectile.transform.position.y + Mathf.Sin((angle * Mathf.PI) / 180f);

        Vector3 bulMoveVector = new Vector3(bulDirX, bulDirY, 0f);
        bulletDirection = (bulMoveVector - currentProjectile.transform.position).normalized;

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

    public void ShootPattern(int index, int pointer)
    {
        switch (weaponList[index].weaponType.pattern)
        {
            case BulletPattern.Straight:
                StraightBullet(index, pointer);
                break;

            case BulletPattern.Radius:
                RadiusBullet(index, pointer);
                break;

            case BulletPattern.Spiral:
                SpiralBullet(index, pointer);
                break;

            case BulletPattern.Random:
                RandomBullet(index, pointer);
                break;

            case BulletPattern.DirectionToPlayer:
                DirectionToPlayerBullet(index, pointer);
                break;
        }
    }


    public void StraightBullet(int i, int pointer)
    {
        Vector2 position = CalculatebulletPosition(i, pointer);
        Debug.Log(position);
        Debug.Log(pointer);
        SetProjectileValues(weaponList[i].gunPosition, position.x, position.y, 90f);
    }

    public void RadiusBullet(int i, int pointer)
    {
        float angleStep = 0f;

        if (weaponList[i].amountToShoot - 1 >= 1)
        {
            angleStep = (weaponList[i].weaponType.endAngle - weaponList[i].weaponType.startAngle) / (weaponList[i].amountToShoot - 1);
        }

        float angle = weaponList[i].weaponType.startAngle + (angleStep * pointer);

        SetProjectileValues(weaponList[i].gunPosition, 0f, 0f, angle);
    }

    public void SpiralBullet(int i, int pointer)
    {
        float angleStep = 0f;
        if (pointer == 0)
        {
            angleStep = weaponList[i].weaponType.increaseAngle + weaponList[i].increaseAngle;
            weaponList[i].increaseAngle = angleStep;
        }
        else
        {
            angleStep = weaponList[i].increaseAngle;
        }

        float angle = 360 / weaponList[i].amountToShoot * pointer + angleStep;
        SetProjectileValues(weaponList[i].gunPosition, 0f, 0f, angle);
    }

    public void RandomBullet(int i, int pointer)
    {
        float angle = Random.Range(0f, 360f);
        SetProjectileValues(weaponList[i].gunPosition, 0f, 0f, angle);
    }

    public void DirectionToPlayerBullet(int i, int pointer)
    {
        Transform enemyPosition = PointTowardsClosestEnemy();
        if (enemyPosition != null)
        {
            Vector3 position = CalculatebulletPosition(i, pointer);

            Vector3 direction = enemyPosition.position - (weaponList[i].gunPosition.position);
            direction.Normalize();
            float rotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            SetProjectileValues(weaponList[i].gunPosition, position.x, position.y, rotation);
        }
        else
        {
            StraightBullet(i, pointer);
        }


    }

    private Vector3 CalculatebulletPosition(int i, int pointer)
    {
        float xPosition = 0f;
        if (weaponList[i].amountToShoot == 1)
        {
            xPosition = 0f;
        }
        else if (weaponList[i].amountToShoot > 1)
        {
            if (weaponList[i].amountToShoot % 2 == 0)
            {
                if (pointer < weaponList[i].amountToShoot / 2)
                {
                    xPosition = weaponList[i].weaponType.initialOffsetX - weaponList[i].weaponType.offsetXBetweenShots * (weaponList[i].amountToShoot / 2 - pointer);
                }
                else
                {
                    xPosition = -weaponList[i].weaponType.initialOffsetX + weaponList[i].weaponType.offsetXBetweenShots * (pointer + 1 - weaponList[i].amountToShoot / 2);
                }
            }
            else
            {
                int centerBullet = Mathf.RoundToInt(weaponList[i].amountToShoot / 2);

                if (pointer < weaponList[i].amountToShoot / 2)
                {
                    xPosition = -weaponList[i].weaponType.offsetXBetweenShots * (weaponList[i].amountToShoot / 2 - pointer);
                }
                else if (pointer == centerBullet)
                {
                    xPosition = 0f;
                }
                else
                {
                    xPosition = weaponList[i].weaponType.offsetXBetweenShots * (pointer - weaponList[i].amountToShoot / 2);
                }
            }
        }

        return new Vector3(xPosition, 0f, 0f);
    }

    protected abstract Transform PointTowardsClosestEnemy();

}

[System.Serializable]
public class WeaponList
{
    public WeaponType weaponType;
    public int amountToShoot;
    public float timeBetweenShots;
    public float timeUntilNextShot;
    public Transform gunPosition;
    [Header("Only used in Spiral Pattern")] [HideInInspector] public float increaseAngle;
}