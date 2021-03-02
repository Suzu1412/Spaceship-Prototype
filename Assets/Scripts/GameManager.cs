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
    [SerializeField] private Text gameOverText;
    [SerializeField] private Text fpsCounter;
    [SerializeField] private Image healthBarP1;
    [SerializeField] Image healthBarP2;
    [SerializeField] private GameObject[] players;
    [SerializeField] private List<ItemPickUp> itemList;
    private Transform path;
    private SceneManager sceneManager;
    private int numberOfEnemies;
    private GameState _state;
    public GameState state { get { return _state; } }
    private bool gameStarted;
    private bool gamePlaying;
    private bool victory;
    private bool gameOver;
    private bool lastWave;
    private ObjectPooler _objectPooler;


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
        victoryText.gameObject.SetActive(false);
        readyText.gameObject.SetActive(false);
        gameOverText.gameObject.SetActive(false);
        _objectPooler = ObjectPooler.Instance;

        for (int i = 0; i < itemList.Count; i++)
        {
            ObjectPooler.Pool item = new ObjectPooler.Pool
            {
                prefab = itemList[i].itemSpawn.gameObject,
                shouldExpandPool = true,
                size = 30,
                tag = itemList[i].itemSpawn.name,
                isChild = true
            };

            _objectPooler.CreatePool(item);
        }

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

        EnablePath();
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

    void EnablePath()
    {
        if  (path == null)
        {
            path = GameObject.Find("Path").transform;
        }

        path.gameObject.SetActive(true);

        for (int i=0; i < path.childCount; i++)
        {
            path.GetChild(i).gameObject.SetActive(true);

            for (int j=0; j < path.GetChild(i).childCount; j++)
            {
                path.GetChild(i).GetChild(j).gameObject.SetActive(true);
            }
        }
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
        if (!gameOver)
        {
            sceneManager.EndScene();
            EnableGameOverText();
            gameOver = true;
        }
        
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

    void EnableGameOverText()
    {
        gameOverText.gameObject.SetActive(true);
        gameOverText.canvasRenderer.SetAlpha(0f);
        FadeOut(gameOverText);
    }

    void DisableGameOverText()
    {
        gameOverText.gameObject.SetActive(false);
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
            bool playerSurvive = false;
            for (int i=0; i < players.Length; i++)
            {
                if (players[i].activeInHierarchy && players[i].GetComponent<PlayerController>().currentHealth > 0)
                {
                    playerSurvive = true;
                }
            }

            if (playerSurvive)
            {
                _state = GameState.Victory;
            }
            else
            {
                _state = GameState.GameOver;
            }
            
        }
    }

    public void LastWave()
    {
        lastWave = true;
    }

    public int GetEnemyCount()
    {
        return numberOfEnemies;
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