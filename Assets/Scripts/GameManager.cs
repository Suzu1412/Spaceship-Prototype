using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-2)]
public class GameManager : MonoBehaviour
{
    [SerializeField] private ScoreManager scorePlayer1;
    [SerializeField] private ScoreManager scorePlayer2;
    [SerializeField] private Text textScorePlayer1;
    [SerializeField] private Text textScorePlayer2;
    [SerializeField] private Text readyText;
    [SerializeField] private Text victoryText;
    [SerializeField] private Text fpsCounter;
    [SerializeField] private Image healthBarP1;
    [SerializeField] Image healthBarP2;
    [SerializeField] private GameObject[] players;
    private SceneManager sceneManager;
    private int numberOfEnemies;
    private GameState _state;
    public GameState state { get { return _state; } }
    private bool gameStarted;
    private bool gamePlaying;
    private bool victory;
    private bool lastWave;


    private void Awake()
    {
        if (scorePlayer1 == null) Debug.LogError("Score player 1 Empty");
        if (scorePlayer2 == null) Debug.LogError("Score player 2 Empty");
        if (textScorePlayer1 == null) Debug.LogError("Text UI Score player 1 Empty");
        if (textScorePlayer2 == null) Debug.LogError("Text UI Score player 2 Empty");
        if (readyText == null) Debug.LogError("Ready Text Empty");

        readyText.gameObject.SetActive(false);
        players = GameObject.FindGameObjectsWithTag("Player");
        sceneManager = GameObject.FindObjectOfType<SceneManager>();
        
        if (players.Length >= 1)
        {
            players[0].GetComponent<CharController>().SetScore(scorePlayer1);
            players[0].GetComponent<PlayerController>().SetHealhBar(healthBarP1);
            scorePlayer1.player = "P1";
            scorePlayer1.scoreText = textScorePlayer1;
            scorePlayer1.UpdateText();

            if (players.Length > 1)
            {
                players[1].GetComponent<CharController>().SetScore(scorePlayer2);
                players[1].GetComponent<PlayerController>().SetHealhBar(healthBarP2);
                scorePlayer2.player = "P2";
                scorePlayer2.scoreText = textScorePlayer2;
                scorePlayer2.UpdateText();
            }
            else
            {
                textScorePlayer2.gameObject.SetActive(false);
                healthBarP2.gameObject.transform.parent.parent.gameObject.SetActive(false);
            }
        }
        else
        {
            //Debug.LogError("No Player found on the Scene");
        }
        #if !UNITY_EDITOR
            _state = GameState.Start;
        #else
            _state = GameState.Playing;
        #endif
        }

    private void Start()
    {
        UpdateState();
        InvokeRepeating("UpdateFPSCounter", 0f, 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateState();
    }

    void UpdateState()
    {
        switch (state)
        {
            case GameState.Start:
                StartGame();
                break;

            case GameState.Playing:
                PlayGame();
                break;

            case GameState.GameOver:
                GameOver();
                break;

            case GameState.Victory:
                Victory();
                break;
        }
    }

    private void StartGame()
    {
        if (!gameStarted)
        {
            for (int i = 0; i < players.Length; i++)
            {
                players[i].GetComponent<PlayerController>().CanShoot(false);
            }

            if (players.Length == 1)
            {
                players[0].transform.position = new Vector2(0f, -7);
            }

            sceneManager.StartScene();

            Invoke("EnableReadyText", 0.6f);
            Invoke("DisableReadyText", 3f);
            Invoke("SetStatePlaying", 3f);

            gameStarted = true;
        }
        
    }

    private void PlayGame()
    {
        if (!gamePlaying)
        {
            for (int i= 0; i < players.Length; i++)
            {
                //players[i].GetComponent<PlayerController>().CanShoot(true);
                Invoke("MakePlayerShoot", 1f);
            }

            gamePlaying = true;
        }
        else
        {
            if (players.Length >= 1)
            {
                if (!players[0].activeSelf)
                {
                    _state = GameState.GameOver;
                }
            }
        }
    }

    void MakePlayerShoot()
    {
        players[0].GetComponent<PlayerController>().CanShoot(true);
    }

    void Victory()
    {
        if (!victory)
        {
            for (int i = 0; i < players.Length; i++)
            {
                players[i].GetComponent<PlayerController>().CanShoot(false);
            }

            sceneManager.EndScene();
            EnableVictoryText();
            victory = true;
        }
        
    }

    void GameOver()
    {
        //Debug.Log("Game Over");
    }

    void EnableReadyText()
    {
        readyText.gameObject.SetActive(true);
        readyText.canvasRenderer.SetAlpha(0f);
        FadeOut(readyText);
    }

    void DisableReadyText()
    {
        readyText.gameObject.SetActive(false);
    }

    void EnableVictoryText()
    {
        victoryText.gameObject.SetActive(true);
        victoryText.canvasRenderer.SetAlpha(0f);
        FadeOut(victoryText);
    }

    void SetStatePlaying()
    {
        gameStarted = false;
        gamePlaying = false;
        victory  = false;
        _state = GameState.Playing;
    }

    void UpdateFPSCounter()
    {
        float fps = Mathf.Round(1f / Time.unscaledDeltaTime);
        fpsCounter.text = "FPS: " + fps.ToString();
    }

    public void AddEnemyCount()
    {
        numberOfEnemies++;
    }

    public void RemoveEnemyCount()
    {
        numberOfEnemies--;

        if (lastWave && numberOfEnemies == 0)
        {
            _state = GameState.Victory;
        }
    }

    public void LastWave()
    {
        lastWave = true;
    }

    void FadeIn(Text text)
    {
        text.CrossFadeAlpha(0f, 0.2f, false);
    }

    void FadeOut(Text text)
    {
        text.CrossFadeAlpha(1f, 0.5f, false);
    }
}

public enum GameState
{
    Start,
    Playing,
    GameOver,
    Victory
}