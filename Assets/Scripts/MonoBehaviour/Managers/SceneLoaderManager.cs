using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoaderManager : MonoBehaviour
{
    [SerializeField] private Image blackFade;

    private void Awake()
    {
        blackFade.canvasRenderer.SetAlpha(0f);
        blackFade.gameObject.SetActive(false);
    }

    public void StartScene()
    {
        blackFade.gameObject.SetActive(true);
        blackFade.canvasRenderer.SetAlpha(1f);
        FadeOut();
    }

    public void EndScene()
    {
        blackFade.gameObject.SetActive(true);
        blackFade.canvasRenderer.SetAlpha(0f);
        FadeIn();
    }

    void FadeIn()
    {
        blackFade.CrossFadeAlpha(1f, 1f, false);
    }

    void FadeOut()
    {
        blackFade.CrossFadeAlpha(0f, 2.5f, false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PauseGame()
    {
        if (Time.timeScale > 0)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
        
    }

    public void RestartScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
