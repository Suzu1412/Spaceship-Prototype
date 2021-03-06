using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-2)]
public class GameManager : MonoBehaviour
{
    [SerializeField] private ScoreManager scorePlayer1;
    [SerializeField] private ScoreManager scorePlayer2;
    [SerializeField] private ScoreManager highScore;
    [SerializeField] private Text textScorePlayer1;
    [SerializeField] private Text textScorePlayer2;
    [SerializeField] private Text textHighScore;
    [SerializeField] private Text readyText;
    [SerializeField] private Text victoryText;
    [SerializeField] private Text gameOverText;
    [SerializeField] private Text fpsCounter;
    private GameObject[] players;
    [SerializeField] private List<ItemPickUp> smallitemList;
    [SerializeField] private List<ItemPickUp> bigItemList;
    [SerializeField] private HealthBarManager playerHealthBar;
    [SerializeField] private HealthBarManager bossHealthBar;
    [SerializeField] private ExperienceManager playerExperience;
    [SerializeField] private List<Explosion> explosions;
    private Transform path;
    private SceneLoaderManager sceneManager;
    private int numberOfEnemies;
    private GameState _state;
    public GameState State { get { return _state; } }
    private bool gameStarted;
    private bool gamePlaying;
    private bool victory;
    private bool gameOver;
    private bool lastWave;
    private ObjectPooler _objectPooler;

    private void Awake()
    {
        SetRatio(9, 16);

        if (scorePlayer1 == null) Debug.LogError("Score player 1 Empty");
        if (scorePlayer2 == null) Debug.LogError("Score player 2 Empty");
        if (textScorePlayer1 == null) Debug.LogError("Text UI Score player 1 Empty");
        if (textScorePlayer2 == null) Debug.LogError("Text UI Score player 2 Empty");
        if (readyText == null) Debug.LogError("Ready Text Empty");
        if (playerHealthBar == null) Debug.LogError("Player health bar not assigned");

        readyText.gameObject.SetActive(false);
        players = GameObject.FindGameObjectsWithTag("Player");
        sceneManager = GameObject.FindObjectOfType<SceneLoaderManager>();
        victoryText.gameObject.SetActive(false);
        readyText.gameObject.SetActive(false);
        gameOverText.gameObject.SetActive(false);
        _objectPooler = ObjectPooler.Instance;

        for (int i = 0; i < smallitemList.Count; i++)
        {
            ObjectPooler.Pool item = new ObjectPooler.Pool
            {
                prefab = smallitemList[i].itemSpawn.gameObject,
                shouldExpandPool = true,
                size = 30,
                tag = smallitemList[i].itemSpawn.name,
                isChild = true
            };

            _objectPooler.CreatePool(item);
        }

        for (int i = 0; i < bigItemList.Count; i++)
        {
            ObjectPooler.Pool item = new ObjectPooler.Pool
            {
                prefab = bigItemList[i].itemSpawn.gameObject,
                shouldExpandPool = true,
                size = 5,
                tag = bigItemList[i].itemSpawn.name,
                isChild = true
            };

            _objectPooler.CreatePool(item);
        }

        for (int i = 0; i < explosions.Count; i++)
        {
            ObjectPooler.Pool item = new ObjectPooler.Pool
            {
                prefab = explosions[i].explosion.gameObject,
                shouldExpandPool = true,
                size = 3,
                tag = explosions[i].explosion.name,
                isChild = true
            };

            _objectPooler.CreatePool(item);
        }

        if (players.Length >= 1)
        {
            players[0].GetComponent<CharController>().SetScore(scorePlayer1);
            players[0].GetComponent<CharController>().SetHighScore(highScore);
            playerHealthBar.SetCharacter(players[0].GetComponent<PlayerController>());
            playerExperience.SetPlayer(players[0].GetComponent<PlayerController>());
            scorePlayer1.ResetValues();
            scorePlayer1.scoreText = textScorePlayer1;
            scorePlayer1.UpdateText();
            SetHighScore();

            if (players.Length > 1)
            {
                players[1].GetComponent<CharController>().SetScore(scorePlayer2);
                //players[1].GetComponent<PlayerController>().SetHealhBar(healthBarP2);
                //scorePlayer2.player = "P2";
                //scorePlayer2.scoreText = textScorePlayer2;
                //scorePlayer2.UpdateText();
            }
            else
            {
                textScorePlayer2.gameObject.SetActive(false);
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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            sceneManager.QuitGame();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            sceneManager.RestartScene();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            sceneManager.PauseGame();
        }
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
        switch (State)
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

                if (players[0].activeSelf)
                {
                    if (players[0].GetComponent<PlayerController>().canShoot == false && !players[0].GetComponent<PlayerController>().isDeath)
                    {
                        Invoke("MakePlayerShoot", 0f);
                    }
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

            Invoke("EndScene", 1f);
            Invoke("EnableVictoryText", 1f);
            victory = true;
            Invoke("StopTime", 2f);
        }
    }

    void EndScene()
    {
        sceneManager.EndScene();
    }

    void StopTime()
    {
        Time.timeScale = 0f;
    }

    void ResumeTime()
    {
        Time.timeScale = 1f;
    }

    void GameOver()
    {
        if (!gameOver)
        {
            sceneManager.EndScene();
            EnableGameOverText();
            gameOver = true;
            //Invoke("StopTime", 1f);
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
        fpsCounter.text = fps.ToString();
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

            players = GameObject.FindGameObjectsWithTag("Player");

            for (int i=0; i < players.Length; i++)
            {
                if (players[i].GetComponent<PlayerController>().currentHealth > 0)
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

    void SetHighScore()
    {
        highScore.ResetValues();
        highScore.scoreText = textHighScore;
        highScore.SetScore(PlayerPrefs.GetInt("HighScore"));
    }

    void SetRatio(float w, float h)
    {
        if ((((float)Screen.width) / ((float)Screen.height)) > w / h)
        {
            Screen.SetResolution((int)(((float)Screen.height) * (w / h)), Screen.height, FullScreenMode.FullScreenWindow);
        }
        else
        {
            Screen.SetResolution(Screen.width, (int)(((float)Screen.width) * (h / w)), FullScreenMode.FullScreenWindow);
        }
    }
}

public enum GameState
{
    Start,
    Playing,
    GameOver,
    Victory
}