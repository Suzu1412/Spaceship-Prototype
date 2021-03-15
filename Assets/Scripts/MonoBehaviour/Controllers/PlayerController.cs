using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : CharController
{
    [Header("Player variables")]
    private InputController _input;
    private CircleCollider2D _magnetCollider;
    private bool _victory;
    private ExperienceManager _experienceBar;
    private bool _levelUp;
    [SerializeField] private SpriteRenderer _shield;
    [SerializeField] private GameObject explosion;
    [SerializeField] private CharacterDescription _description;
    public List<SpecialEffectSO> effectList;
    public List<TemporaryBuff> buffList;

    [Header("Player Start Animation")]
    public float smoothTimeStart = 0.6F;
    public float smoothTimeEnd = 0.6F;
    public float smoothVictory = 1.2f;
    bool arrivedAtStartPosition;
    private Vector3 startMarker;
    private Vector3 endMarker;
    private Vector3 velocity = Vector3.zero;

    [Header("Player Attributes")]
    [SerializeField] private PlayerStats _stats;
    private int _currentLevel;

    [Header("Decision Making")]
    private bool _canShoot = false;

    [Header("Movement Constraint")]
    private float rightCorner;
    private float topCorner;
    [SerializeField] private float offset;

    [Header("Shield")]
    private float _shieldResistance;
    private bool _shieldActive = false;

    public InputController Input { get { return _input; } }
    public bool canShoot { get { return _canShoot; } }
    public PlayerStats stats { get { return _stats; } }
    public bool victory { get { return _victory; } }
    public bool LevelUp { get { return _levelUp; } }
    public ExperienceManager ExperienceBar { get { return _experienceBar; } }
    public CharacterDescription Description { get { return _description; } }
    public SpriteRenderer Sprite { get { return _sprite; } }

    protected override void Awake()
    {
        base.Awake();
        explosion.SetActive(false);
        _input = GetComponent<InputController>();
        _magnetCollider = GetComponentInChildren<CircleCollider2D>();
        if (_input == null) Debug.LogError(this.gameObject.name + " missing InputController");
        if (_stats == null) Debug.Log(this.gameObject.name + " missing Stats");
        _stats.ResetValues();
        _victory = false;
        _shield.gameObject.SetActive(false);
    }

    public void Shoot()
    {
        _canShoot = true;
    }

    public void CanShoot(bool canShoot)
    {
        _canShoot = canShoot;
    }

    // Start is called before the first frame update
    protected void Start()
    {
        rightCorner = 2.8f;
        topCorner = 5f;
        if (offset == 0) offset = 0.2f;
        arrivedAtStartPosition = false;
        startMarker = new Vector3(this.transform.position.x, -1.5f, 0f);
        endMarker = new Vector3(this.transform.position.x, -3.2f, 0f);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _maxHealth = stats.maxHealth;
        _currentHealth = _maxHealth;
        float fillAmount = (float)currentHealth / (float)stats.maxHealth;
        _isDeath = false;
        _invulnerable = false;
        DisableShield();
    }

    protected void OnDisable()
    {
        Input.DisableTouchJoystick();
    }

    // Update is called once per frame
    void Update()
    {
        switch(_manager.State){
            case GameState.Start:
                MoveToStartPosition();
                break;

            case GameState.Playing:
                LimitBreakStats();
                RestrictMovement();
                break;

            case GameState.Victory:
                Input.DisableTouchJoystick();
                Victory();
                break;

            case GameState.GameOver:
                Input.DisableTouchJoystick();
                break;
        }
    }

    private void FixedUpdate()
    {
        if (_manager.State == GameState.Playing)
        {
            if (!isDeath)
            {
                Move();
            }
            else
            {
                rb.velocity = Vector2.zero;
            }
        }
    }

    private void Move()
    {
        Vector2 movement = new Vector2(Input.Horizontal * stats.moveSpeed, Input.Vertical * stats.moveSpeed);
        _rb.velocity = movement;
    }

    private void RestrictMovement()
    {
        if (transform.position.x <= -rightCorner + offset) 
            transform.position = new Vector3(-rightCorner + offset, transform.position.y);

        if (transform.position.x >= rightCorner - offset)
            transform.position = new Vector3(rightCorner - offset, transform.position.y);

        if (transform.position.y <= -topCorner + offset)
            transform.position = new Vector3(transform.position.x, -topCorner + offset);

        if (transform.position.y >= topCorner - offset)
            transform.position = new Vector3(transform.position.x, topCorner - offset);
    }

    #region Implementing Ihealth
    public override void Heal(int amount)
    {
        if (isDeath) return;

        _healthBar.ResetHeal();
        _healed = true;
        if (_currentHealth + amount > stats.maxHealth)
        {
            _currentHealth = stats.maxHealth;
        }
        else
        {
            _currentHealth += amount;
        }
    }

    public override void Damage(int amount)
    {
        if (isDeath) return;
        if (isInvulnerable) return;
        if (_shieldActive)
        {
            _shieldResistance -= 1;

            if (_shieldResistance <= 0)
            {
                DisableShield();
            }

            return;
        }

        _healthBar.ResetDamage();
        MaxLevelExperience(amount);
        _damaged = true;
        
        if (_currentHealth - amount <= 0)
        {
            _currentHealth = 0;
            Death();
        }
        else
            {
            Invoke("Invulnerability", 0f);
            Invoke("RemoveInvulnerability", 1.5f);
            _currentHealth -= amount;
        }
    }

    public override void Death()
    {
        if (!isDeath)
        {
            _canShoot = false;
            _collider.enabled = false;
            _sprite.enabled = false;
            GameObject obj = Instantiate(explosion);
            obj.transform.position = this.transform.position;
            obj.SetActive(true);
            _isDeath = true;
            Invoke("SetActiveFalse", 1f);
        }
    }
    #endregion

    public bool IsShooting()
    {
        return Input.IsShooting;
    }

    /// <summary>
    /// Move Upwards the screen in certain amount of seconds. 
    /// After it has reached the max Position, slowly move downwards to position himself in the end Marker
    /// </summary>
    void MoveToStartPosition()
    {
        if (!arrivedAtStartPosition)
        {
            Vector3 targetPosition = startMarker;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTimeStart);
            if (Vector3.Distance(transform.position, startMarker) < 0.2f) arrivedAtStartPosition = true;
        }
        else
        {
            Vector3 targetPosition = endMarker;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTimeEnd);
        }
    }

    #region LevelUp
    public void AddExperience(int amount)
    {
        int newLevel = _stats.AddExperience(amount);

        ExperienceBar.UpdateExpBar();
        if (_currentLevel < newLevel)
        {
            _levelUp = true;
        }
        _currentLevel = newLevel;
    }

    /// <summary>
    /// If Player has reached Limit break, reduce the duration of limit break.
    /// If Player has reached Max Level, the damage received will help him to reach the Limit Break
    /// </summary>
    /// <param name="amount">The damage amount received by the player Overcharges the limit break bar</param>
    public void MaxLevelExperience(int amount)
    {
        if (stats.LimitBreak)
        {
            stats.DamageLimitBreak();
        }
        else if (stats.MaxLevel)
        {
            int newLevel = _stats.AddExperience(amount);

            ExperienceBar.UpdateExpBar();
            if (_currentLevel < newLevel)
            {
                _levelUp = true;
            }
            _currentLevel = newLevel;
        }
    }
    #endregion

    void Victory()
    {
        _invulnerable = true;
        _magnetCollider.radius = 10f;
        _victory = true;
        rb.velocity = Vector2.up * 5f;
    }

    void Invulnerability()
    {
        _invulnerable = true;
        _sprite.color = new Color(_sprite.color.r, _sprite.color.g, _sprite.color.b, 0.3f);
    }

    void RemoveInvulnerability()
    {
        _invulnerable = false;
        _sprite.color = new Color(_sprite.color.r, _sprite.color.g, _sprite.color.b, 1f);
    }

    public void AddScore(int amount)
    {
        score.SetScore(amount);

        if (highScore.GetScore() <= score.GetScore())
        {
            highScore.SetScore(score.GetScore() - highScore.GetScore());
            PlayerPrefs.SetInt("HighScore", score.GetScore());
        }
    }

    public void SetExperienceBar(ExperienceManager experience)
    {
        _experienceBar = experience;
    }

    public void LimitBreakStats()
    {
        if (stats.LimitBreak )
        {
            _magnetCollider.radius = 1.5f;
        }
        else
        {
            _magnetCollider.radius = 1f;
        }
    }

    public void EndLevelUp()
    {
        _levelUp = false;
    }

    public void ActivateShield(int amount)
    {
        _shieldActive = true;
        if (_shieldResistance < amount)
        {
            _shieldResistance = amount;
        }
        _shield.gameObject.SetActive(true);
    }

    public void DisableShield()
    {
        _shieldActive = false;
        _shieldResistance = 0;
        _shield.gameObject.SetActive(false);
    }

    public void AddBuff(TemporaryBuff buff)
    {
        buffList.Add(buff);
    }

    public void RemoveBuff(TemporaryBuff buff)
    {
        buffList.Remove(buff);
    }

    public IEnumerator BuffDuration(TemporaryBuff buff)
    {
        bool isAdded = false;
        buff.Initialize(this);
        buffList.Add(buff);

        if (!buff.permanent)
        {
            yield return new WaitForSeconds(buff.duration);
            buffList.Remove(buff);
            for (int i = 0; i < buffList.Count; i++)
            {
                if (buffList[i] == buff)
                {
                    isAdded = true;
                    break;
                }
            }

            if (!isAdded)
            {
                buff.RemoveBuff(this);
            }
        }
    }
}

[System.Serializable]
public class CharacterDescription{
    public string name;
    public StatDescription attack;
    public StatDescription attackSpeed;
    public StatDescription movementSpeed;
    public StatDescription health;
    public SpecialDescription special;

    public enum StatDescription
    {
        Lowest,
        Low,
        Normal,
        High,
        Max
    }

    public enum SpecialDescription
    {
        None,
        Homing,
        Charge,
        Pierce,
        Bounce,
        Instakill,
        Lifesteal,
        Boomerang,
        Freeze,
        Burn,
        Cut,
        Spread,
        Dark,
        Explode,
        Drone
    }

}
