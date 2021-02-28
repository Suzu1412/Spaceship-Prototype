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
    [SerializeField] private Text fpsCounter;
    [SerializeField] private Image healthBarP1;
    [SerializeField] Image healthBarP2;
    [SerializeField] private GameObject[] players;
    private GameState _state;
    public GameState state { get { return _state; } }
    private bool gameStarted;
    private bool gamePlaying;
    private bool finishedGame;


    private void Awake()
    {
        if (scorePlayer1 == null) Debug.LogError("Score player 1 Empty");
        if (scorePlayer2 == null) Debug.LogError("Score player 2 Empty");
        if (textScorePlayer1 == null) Debug.LogError("Text UI Score player 1 Empty");
        if (textScorePlayer2 == null) Debug.LogError("Text UI Score player 2 Empty");
        if (readyText == null) Debug.LogError("Ready Text Empty");

        readyText.gameObject.SetActive(false);
        players = GameObject.FindGameObjectsWithTag("Player");
        
        if (players != null)
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
            Debug.LogError("No Player found on the Scene");
        }
        _state = GameState.Playing;
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

            Invoke("EnableReadyText", 0.8f);
            Invoke("DisableReadyText", 3f);
            Invoke("SetStatePlaying", 3f);

            gameStarted = true;
        }
        
    }

    private void PlayGame()
    {
        if (!gamePlaying)
        {
            Debug.Log("Play Game");
            for (int i= 0; i < players.Length; i++)
            {
                players[i].GetComponent<PlayerController>().CanShoot(true);
            }

            gamePlaying = true;
        }
        else
        {
            if (players[0].activeSelf == false)
            {
                _state = GameState.GameOver;
            }
        }
    }

    void GameOver()
    {
        Debug.Log("Game Over");
    }

    void EnableReadyText()
    {
        readyText.gameObject.SetActive(true);
    }

    void DisableReadyText()
    {
        readyText.gameObject.SetActive(false);
    }

    void SetStatePlaying()
    {
        gameStarted = false;
        gamePlaying = false;
        finishedGame = false;
        _state = GameState.Playing;
    }

    void UpdateFPSCounter()
    {
        float fps = Mathf.Round(1f / Time.unscaledDeltaTime);
        fpsCounter.text = "FPS: " + fps.ToString();
    }
}

public enum GameState
{
    Start,
    Playing,
    GameOver,
    Victory
}