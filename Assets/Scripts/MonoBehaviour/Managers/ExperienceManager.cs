using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExperienceManager : MonoBehaviour
{
    private PlayerController player;
    [SerializeField] private Image expBar;
    [SerializeField] private Image limitBreakBar;
    private bool _expBarAssiged = false;
    private bool reachedMaxLevel;
    private float lerpTimer;
    private float delayTimer;
    Color originalColor = new Color(0, 0.3f, 1f, 1);
    Color flashColor = new Color(0, 0.6f, 1f, 1);

    private void Start()
    {
        SetExperienceBar();
    }

    private void Update()
    {
        MaxLevelAnimation();
        SetLimitBreakBar();
    }

    private void FixedUpdate()
    {
        player.stats.ReleaseLimitBreak();
    }

    public void SetPlayer(PlayerController player)
    {
        this.player = player;
        this.player.SetExperienceBar(this);
    }

    public void SetExperienceBar()
    {

        if (_expBarAssiged && player.isActiveAndEnabled) return;

        if (!player.isActiveAndEnabled) _expBarAssiged = false;
        
        if (player.maxHealth != 0 && !_expBarAssiged && player.isActiveAndEnabled)
        {
            expBar.fillAmount = (float) player.stats.Experience / player.stats.ExperienceToNextLevel;
            _expBarAssiged = true;
        }
    }

    public void SetLimitBreakBar()
    {
        limitBreakBar.fillAmount = player.stats.LimitBreakDuration / player.stats.ExperienceToNextLevel;
    }

    public void UpdateExpBar()
    {
        expBar.fillAmount = (float) player.stats.Experience / player.stats.ExperienceToNextLevel;
    }

    void MaxLevelAnimation()
    {
        if (player.stats.LimitBreak)
        {
            delayTimer -= Time.deltaTime;

            if (delayTimer <= 0)
            {
                lerpTimer += Time.deltaTime;
                float percentComplete = lerpTimer;
                percentComplete = percentComplete * percentComplete;
                limitBreakBar.color = Color.Lerp(limitBreakBar.color, flashColor, percentComplete);

                if (limitBreakBar.color == flashColor)
                {
                    delayTimer = 1f;
                    lerpTimer = 0f;
                    limitBreakBar.color = originalColor;
                }
            }
        }  
        else
        {
            limitBreakBar.color = originalColor;
        }
    }
}
