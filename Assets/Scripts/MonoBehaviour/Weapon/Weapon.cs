using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] protected List<WeaponList> weaponList;
    protected GameObject currentProjectile;
    protected ObjectPooler objectPooler;
    protected float timeUntilNextShoot;
    private Vector3 bulMoveVector;
    private Vector2 bulletDirection;
    private float bulDirX;
    private float bulDirY;

    protected virtual void Awake()
    {
        objectPooler = ObjectPooler.Instance;

        for (int i = 0; i < weaponList.Count; i++)
        {
            ObjectPooler.Pool item = new ObjectPooler.Pool
            {
                prefab = weaponList[i].weaponType.projectile,
                shouldExpandPool = true,
                size = weaponList[i].weaponType.amountToPool,
                tag = weaponList[i].weaponType.projectile.name,
                isChild = true
            };
            objectPooler.CreatePool(item);
        }
    }

    protected void OnEnable()
    {
        for (int i=0; i < weaponList.Count; i++)
        {
            weaponList[i].increaseAngle = 0f; //weaponList[i].weaponType.increaseAngle;
            weaponList[i].timeUntilNextShot = 0f;
        }
    }

    protected virtual void Update()
    {
        timeUntilNextShoot -= Time.deltaTime;
    }

    protected abstract void FireWeapon();

    public virtual void SetProjectileValues(Transform gunPosition, float xPos, float yPos, float angle, int currentWeapon)
    {
        currentProjectile.transform.position = new Vector3(gunPosition.position.x + xPos, gunPosition.position.y + yPos, 0);

        bulDirX = currentProjectile.transform.position.x + Mathf.Cos(angle * Mathf.PI / 180f);
        bulDirY = currentProjectile.transform.position.y + Mathf.Sin(angle * Mathf.PI / 180f);

        bulMoveVector = new Vector3(bulDirX, bulDirY, 0f);
        bulletDirection = (bulMoveVector - currentProjectile.transform.position).normalized;

        currentProjectile.GetComponent<Projectile>().fired = true;
        currentProjectile.GetComponent<Projectile>().projectileLifeTime = weaponList[currentWeapon].weaponType.lifeTime;
        currentProjectile.GetComponent<Projectile>().projectileSpeed = weaponList[currentWeapon].projectileSpeed;
        currentProjectile.GetComponent<Projectile>().SetMoveDirection(bulletDirection);
        currentProjectile.SetActive(true);
    }

    #region ShootPattern
    public void ShootPattern(int index, int pointer)
    {
        switch (weaponList[index].pattern)
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
                DirectionTowardsEnemyBullet(index, pointer);
                break;
        }
    }


    public void StraightBullet(int i, int pointer)
    {
        Vector2 position = CalculatebulletPosition(i, pointer);
        SetProjectileValues(weaponList[i].gunPosition, position.x, position.y, 90f, i);
    }

    public void RadiusBullet(int i, int pointer)
    {
        float angleStep = 0f;
        float angle = 0f;

        if (weaponList[i].amountToShoot >= 2)
        {
            angleStep = (weaponList[i].weaponType.endAngle - weaponList[i].weaponType.startAngle) / (weaponList[i].amountToShoot);

            angle = weaponList[i].weaponType.startAngle + (angleStep * pointer);
        }
        else
        {
            angle = 180f;
        }

        SetProjectileValues(weaponList[i].gunPosition, 0f, 0f, angle  - 90, i);
    }

    public void SpiralBullet(int i, int pointer)
    {
        float angleStep = 0f;
        if (pointer == 0)
        {
            angleStep = weaponList[i].increaseAngle;
            weaponList[i].increaseAngle = weaponList[i].weaponType.increaseAngle + weaponList[i].increaseAngle;
        }
        else
        {
            angleStep = weaponList[i].increaseAngle;
        }

        float angle = 360 / weaponList[i].amountToShoot * pointer + angleStep + 90;
        SetProjectileValues(weaponList[i].gunPosition, 0f, 0f, angle, i);
    }

    public void RandomBullet(int i, int pointer)
    {
        float angle = Random.Range(0f, 360f);
        SetProjectileValues(weaponList[i].gunPosition, 0f, 0f, angle, i);
    }

    protected abstract void DirectionTowardsEnemyBullet(int i, int pointer);

    protected Vector3 CalculatebulletPosition(int i, int pointer)
    {
        float xPosition = 0f;
        float yPosition = 0f;
        if (weaponList[i].amountToShoot == 1)
        {
            xPosition = 0f;
            yPosition = 0f;
        }
        else if (weaponList[i].amountToShoot > 1)
        {
            if (weaponList[i].amountToShoot % 2 == 0)
            {
                if (pointer < weaponList[i].amountToShoot / 2)
                {
                    xPosition = weaponList[i].weaponType.initialOffsetX - weaponList[i].weaponType.offsetXBetweenShots * (weaponList[i].amountToShoot / 2 - pointer);
                    yPosition = weaponList[i].weaponType.initialOffsetY;
                }
                else
                {
                    xPosition = -weaponList[i].weaponType.initialOffsetX + weaponList[i].weaponType.offsetXBetweenShots * (pointer + 1 - weaponList[i].amountToShoot / 2);
                    yPosition = weaponList[i].weaponType.initialOffsetY;
                }
            }
            else
            {
                int centerBullet = Mathf.RoundToInt(weaponList[i].amountToShoot / 2);

                if (pointer < weaponList[i].amountToShoot / 2)
                {
                    xPosition = -weaponList[i].weaponType.offsetXBetweenShots * (weaponList[i].amountToShoot / 2 - pointer);
                    yPosition = -weaponList[i].weaponType.offsetYBetweenShots * (weaponList[i].amountToShoot / 2 - pointer);
   
                }
                else if (pointer == centerBullet)
                {
                    xPosition = 0f;
                    yPosition = 0f;
                }
                else
                {
                    xPosition = weaponList[i].weaponType.offsetXBetweenShots * (pointer - weaponList[i].amountToShoot / 2);
                    yPosition = -weaponList[i].weaponType.offsetYBetweenShots * (pointer - weaponList[i].amountToShoot / 2);
                }
            }
        }
        return new Vector3(xPosition, yPosition, 0f);
    }

    protected abstract Transform PointTowardsClosestEnemy();
    #endregion
}

[System.Serializable]
public class WeaponList
{
    public WeaponType weaponType;
    public int amountToShoot = 1;
    public float projectileSpeed = 80;
    public float timeBetweenShots;
    [HideInInspector] public float timeUntilNextShot;
    public Transform gunPosition;
    public BulletPattern pattern;
    [Header("Only used in Spiral Pattern")] [HideInInspector] public float increaseAngle;
}

public enum BulletPattern
{
    Straight,
    Radius,
    Spiral,
    Random,
    DirectionToPlayer
}