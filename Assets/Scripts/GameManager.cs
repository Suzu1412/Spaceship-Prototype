using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-2)]
public class GameManager : MonoBehaviour
{
    [SerializeField] private ScoreManager scorePlayer1;
    [SerializeField] private ScoreManager scorePlayer2;
    public Text textScorePlayer1;
    public Text textScorePlayer2;
    public Text readyText;
    private GameObject[] players;
    public GameState state;
    bool gameStarted;
    bool gamePlaying;
    bool finishedGame;


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
            scorePlayer1.player = "P1";
            scorePlayer1.scoreText = textScorePlayer1;
            scorePlayer1.UpdateText();

            if (players.Length > 1)
            {
                players[1].GetComponent<CharController>().SetScore(scorePlayer2);
                scorePlayer2.player = "P2";
                scorePlayer2.scoreText = textScorePlayer2;
                scorePlayer2.UpdateText();
            }
            else
            {
                textScorePlayer2.gameObject.SetActive(false);
            }
        }
        else
        {
            Debug.LogError("No Player found on the Scene");
        }
        state = GameState.Playing;
    }

    private void Start()
    {
        UpdateState();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log((int)(1f / Time.unscaledDeltaTime));
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
        state = GameState.Playing;
    }
}

public enum GameState
{
    Start,
    Playing,
    GameOver
}