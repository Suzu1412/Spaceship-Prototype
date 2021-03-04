using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarManager : MonoBehaviour
{
    private CharController character;
    [SerializeField] private Image healthBar;
    [SerializeField] private Image healedAmount;
    [SerializeField] private Image damagedAmount;
    [SerializeField] private Image backHealthBar;
    private bool healthBarAssigned = false;

    [Header("Chip")]
    [SerializeField] float chipSpeed = 2f;
    [SerializeField] float maxDelayChipTimer = 0.5f;

    [Header("Damage Anim")]
    float damageDelay = 0.5f;
    float damageLerpTimer;

    [Header("Heal anim")]
    float healDelay = 0.5f;
    float healLerpTimer;

    [Header("Danger anim")]
    float dangerLerpTimer;
    bool dangerAnim;
    Color originalColor = Color.black;
    Color dangerColor = Color.red;


    private void Start()
    {
        SetHealthBar();
    }

    // Update is called once per frame
    void Update()
    {
        if (character.isHealed || character.isDamaged) { 
            UpdateHealthBar();
        }

        DangerAnimation();
    }

    public void SetCharacter(CharController character)
    {
        this.character = character;
        this.character.SetHealthBar(this);
    }

    public void SetHealthBar()
    {
        if (healthBarAssigned && character.isActiveAndEnabled) return;

        if (!character.isActiveAndEnabled) healthBarAssigned = false;
        
        if (character.maxHealth != 0 && !healthBarAssigned && character.isActiveAndEnabled)
        {
            healthBar.fillAmount = (float)character.currentHealth / (float)character.maxHealth;
            damagedAmount.fillAmount = 0f;
            healedAmount.fillAmount = 0f;
            healthBarAssigned = true;
        }
    }

    void DangerAnimation()
    {
        if (character.currentHealth <= character.maxHealth * 0.3)
        {
            if (character.currentHealth == 0)
            {
                backHealthBar.color = originalColor; 
                return;
            }

            if (!dangerAnim)
            {
                dangerLerpTimer = 0f;
                backHealthBar.color = originalColor;
                dangerAnim = true;
            }

            if (dangerAnim)
            {
                dangerLerpTimer += Time.deltaTime;
                float percentCompleteDanger = dangerLerpTimer / 4;
                percentCompleteDanger = percentCompleteDanger * percentCompleteDanger;
                backHealthBar.color = Color.Lerp(backHealthBar.color, dangerColor, percentCompleteDanger);

                if (backHealthBar.color == dangerColor)
                {
                    dangerAnim = false;
                }
            }
        }
        else
        {
            backHealthBar.color = originalColor;
            dangerAnim = false;
        }
    }

    void UpdateHealthBar()
    {
        float fillAmount = (float)character.currentHealth / (float)character.maxHealth;

        if (character.isDeath)
        {
            damageDelay = 0f;
            if (!character.isActiveAndEnabled) healthBarAssigned = false;
        }

        if (character.isDamaged)
        {
            DamageAnimation(fillAmount);
        }

        if (character.isHealed)
        {
            HealedAnimation(fillAmount);
        }
    }

    public void ResetDamage()
    {
        damageLerpTimer = 0f;
        damageDelay = maxDelayChipTimer;
    }

    public void ResetHeal()
    {
        healLerpTimer = 0f;
        healDelay = maxDelayChipTimer;
    }

    private void DamageAnimation(float fillAmount)
    {
        if (damagedAmount.fillAmount < healthBar.fillAmount) damagedAmount.fillAmount = healthBar.fillAmount;

        if (!character.isHealed) healthBar.fillAmount = fillAmount; //Remueve la salud tras ser atacado. A menos de que el player se haya curado.

        damageDelay -= Time.deltaTime;
        if (damageDelay <= 0)
        {
            damageLerpTimer += Time.deltaTime;
            float percentCompleteDamage = damageLerpTimer / chipSpeed;
            percentCompleteDamage = percentCompleteDamage * percentCompleteDamage; //Al usar la potencia, se crea un efecto que hace que se mueva mas rapido segun pasa el tiempo
            damagedAmount.fillAmount = Mathf.Lerp(damagedAmount.fillAmount, fillAmount, percentCompleteDamage);

            if (percentCompleteDamage == 100)
            {
                damagedAmount.fillAmount = 0f;
                character.SetDamageFalse();
            }
        }
    }

    private void HealedAnimation(float fillAmount)
    {
        if (healedAmount.fillAmount < fillAmount) healedAmount.fillAmount = fillAmount; //Si barra de curacion es menor quiere decir que ha aumentado la cantidad.

        if (character.isDamaged) healedAmount.fillAmount = fillAmount; //Actualiza la barra de salud con el total tras recibir danio

        healDelay -= Time.deltaTime;
        if (healDelay <= 0)
        {
            healLerpTimer += Time.deltaTime;

            float percentCompleteHeal = healLerpTimer / chipSpeed;
            percentCompleteHeal = percentCompleteHeal * percentCompleteHeal;
            healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, healedAmount.fillAmount, percentCompleteHeal);

            if (percentCompleteHeal == 100)
            {
                healedAmount.fillAmount = 0;
                character.SetHealedFalse();
            }
        }
    }
}
