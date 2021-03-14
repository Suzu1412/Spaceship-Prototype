using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(10)]
public class ExperienceManager : MonoBehaviour
{
    private PlayerController player;
    [SerializeField] private TextMeshProUGUI levelUpText;
    [SerializeField] private TextMeshProUGUI currentLevelText;
    [SerializeField] private Image expBar;
    [SerializeField] private Image limitBreakBar;
    private bool _expBarAssiged = false;
    private bool reachedMaxLevel;
    private float lerpTimer;
    private float delayTimer;
    Color originalColor = new Color(0, 0.3f, 1f, 1);
    Color flashColor = new Color(0, 0.6f, 1f, 1);

    private float lerpLimitBreakTimer;
    private float delayLimitBreak;
    Color limitBreakColor = new Color(0f, 0.6f, 1f, 1f);
    private bool reachedLimitBreakColor;

    private void Start()
    {
        levelUpText.gameObject.SetActive(false);
        SetExperienceBar();
        currentLevelText.text = player.stats.Level.ToString();
        currentLevelText.color = Color.white;
    }

    private void Update()
    {
        if (player != null)
        {
            LevelUpAnimation();
            MaxLevelAnimation();
            SetLimitBreakBar();
        }
    }

    private void FixedUpdate()
    {
        if (player != null)
        {
            player.stats.ReleaseLimitBreak();
        }
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

    /// <summary>
    /// Reaching Limit break displays a flashing animation on the player Bar and flash the Level counter on the left of the bar.
    /// Else reset the values of colors of both bar and Level counter to the original
    /// To increase the duration of the Lerp, divide it by a higher number, like this:
    /// float percentComplete = lerpTimer / 2;
    /// </summary>
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

            if (!reachedLimitBreakColor)
            {

                lerpLimitBreakTimer += Time.deltaTime;
                float percentCompleteLimitBreak = lerpLimitBreakTimer;
                percentCompleteLimitBreak = percentCompleteLimitBreak * percentCompleteLimitBreak;
                currentLevelText.color = Color.Lerp(currentLevelText.color, limitBreakColor, percentCompleteLimitBreak);

                if (currentLevelText.color == limitBreakColor)
                {
                    lerpLimitBreakTimer = 0f;
                    reachedLimitBreakColor = true;
                }
            }
            else
            {
                lerpLimitBreakTimer += Time.deltaTime;
                float percentCompleteLimitBreak = lerpLimitBreakTimer;
                percentCompleteLimitBreak = percentCompleteLimitBreak * percentCompleteLimitBreak;
                currentLevelText.color = Color.Lerp(currentLevelText.color, Color.white, percentCompleteLimitBreak);

                if (currentLevelText.color == Color.white)
                {
                    lerpLimitBreakTimer = 0f;
                    reachedLimitBreakColor = false;
                }
            }
            
        }  
        else
        {
            reachedLimitBreakColor = false;
            limitBreakBar.color = originalColor;
            currentLevelText.color = Color.white;
        }
    }

    /// <summary>
    /// Displays a Text that says 'LEVEL UP!' and flash until it disappear. Change duration changing the Wait for seconds. 
    /// </summary>
    /// <returns></returns>
    IEnumerator LevelUpText()
    {
        levelUpText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        levelUpText.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.25f);
        levelUpText.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.25f);
        levelUpText.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.15f);
        levelUpText.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.15f);
        levelUpText.gameObject.SetActive(false);
    }

    void LevelUpAnimation()
    {
        if (player.LevelUp)
        {
            if (player.stats.LimitBreak)
            {
                levelUpText.text = "LIMIT BREAK!";
            }
            else if (player.stats.MaxLevel)
            {
                levelUpText.text = "MAX LEVEL!";
                currentLevelText.text = player.stats.Level.ToString();
            }
            else
            {
                levelUpText.text = "LEVEL UP!";
                currentLevelText.text = player.stats.Level.ToString();
            }
            StartCoroutine(LevelUpText());
            player.EndLevelUp();
        }
    }
}
