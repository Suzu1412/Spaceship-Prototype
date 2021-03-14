using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoaderManager : MonoBehaviour
{
    [SerializeField] private Image blackFade;
    [SerializeField] private Image pauseButton;
    [SerializeField] private Image quitButton;

    [SerializeField] private SpriteRenderer background1;
    [SerializeField] private SpriteRenderer background2;
    [SerializeField] private SpriteRenderer stars1;
    [SerializeField] private SpriteRenderer stars2;
    [SerializeField] PlayerPreferences playerPreferences;
    private bool pausedGame;
    private bool quitGame;

    public bool isPaused { get { return pausedGame; } }
    public bool isQuitting { get { return quitGame; } }
    public PlayerPreferences PlayerPreferences { get { return playerPreferences; } }

    private void Awake()
    {
        if (blackFade != null)
        {
            blackFade.canvasRenderer.SetAlpha(0f);
            blackFade.gameObject.SetActive(false);
        }

        if (pauseButton != null)
        {
            pauseButton.gameObject.SetActive(false);
        }

        if (quitButton != null)
        {
            quitButton.gameObject.SetActive(false);
        }
    }

    public void CharacterSelection()
    {
        SceneManager.LoadScene("CharacterSelect");
    }

    public void FirstStage()
    {
        Time.timeScale = 1f;
        
        SceneTransition();
        Invoke("SelectStage", 0.5f);
    }

    void SelectStage()
    {
        SceneManager.LoadScene("FirstLevel");
        playerPreferences.SetScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void StartScene()
    {
        if (blackFade != null)
        {
            SetBackgroundOnBottom();
            blackFade.gameObject.SetActive(true);
            blackFade.canvasRenderer.SetAlpha(1f);
            FadeOut();
        }
    }

    public void EndScene()
    {
        if (blackFade != null)
        {
            blackFade.gameObject.SetActive(true);
            blackFade.canvasRenderer.SetAlpha(0f);
            FadeIn();
        }
    }

    public void SceneTransition()
    {
        if (blackFade != null)
        {
            blackFade.gameObject.SetActive(true);
            blackFade.canvasRenderer.SetAlpha(0f);
            FadeInFast();
        }
    }

    void FadeIn()
    {
        blackFade.CrossFadeAlpha(1f, 1f, false);
    }

    void FadeInFast()
    {
        blackFade.CrossFadeAlpha(1f, 0.5f, false);
    }

    void FadeOut()
    {
        blackFade.CrossFadeAlpha(0f, 2.5f, false);
    }

    public void QuitPrompt()
    {
        if (Time.timeScale > 0 || !quitGame)
        {
            quitGame = true;
            Time.timeScale = 0f;
            SetBackgroundOnTop();
            pauseButton.gameObject.SetActive(false);
            quitButton.gameObject.SetActive(true);
        }
        else
        {
            if (pausedGame)
            {
                quitGame = false;
                pauseButton.gameObject.SetActive(true);
                quitButton.gameObject.SetActive(false);
            }
            else
            {
                quitGame = false;
                Time.timeScale = 1f;
                SetBackgroundOnBottom();
                quitButton.gameObject.SetActive(false);
            }
        }
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public void PauseGame()
    {
        if (Time.timeScale > 0)
        {
            pausedGame = true;
            Time.timeScale = 0f;
            SetBackgroundOnTop();
            pauseButton.gameObject.SetActive(true);
        }
        else
        {
            pausedGame = false;
            Time.timeScale = 1f;
            SetBackgroundOnBottom();
            pauseButton.gameObject.SetActive(false);
        }
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void SetBackgroundOnTop()
    {
        if (background1 != null && background2 != null && stars1 != null && stars2 != null)
        {
            background1.sortingLayerName = "BackgroundOnTop";
            background2.sortingLayerName = "BackgroundOnTop";
            stars1.sortingLayerName = "BackgroundOnTop";
            stars2.sortingLayerName = "BackgroundOnTop";
        }
    }

    private void SetBackgroundOnBottom()
    {
        if (background1 != null && background2 != null && stars1 != null && stars2 != null)
        {
            background1.sortingLayerName = "Background";
            background2.sortingLayerName = "Background";
            stars1.sortingLayerName = "Background";
            stars2.sortingLayerName = "Background";
        }
    }

    public void RestartScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextScene()
    {
        if (SceneManager.sceneCount < PlayerPreferences.GetScene())
        {
            SceneManager.LoadScene(PlayerPreferences.GetScene());
        }
        else
        {
            MainMenu();
        }
        
    }

    public GameObject SpawnPlayer()
    {
        return Instantiate(playerPreferences.GetCharacter().gameObject, transform.position, Quaternion.identity);
    }
}
