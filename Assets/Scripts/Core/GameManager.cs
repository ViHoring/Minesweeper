using System;
using TMPro;
using Unity.VectorGraphics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Controla o estado do jogo
    //Recebe eventos vindo da UI
    //Chama o BoardController
    //Dispara eventos globais

    [SerializeField] GameObject _mainMenu;
    [SerializeField] GameObject _hud;
    [SerializeField] GameObject _pauseMenu;
    [SerializeField] GameObject _gameOverMenu;
    [SerializeField] TMP_Text _gameOverMsg;
    [SerializeField] Camera _camera;
    [SerializeField] BoardController _boardController;
    [SerializeField] GameObject _hudEdgeR;
    [SerializeField] GameObject _hudEdgeL;
    [SerializeField] TMP_Text _timerText;
    bool _isFirstClick = true;
    BoardConfigSO _config;
    int _totalTiles;
    float _elapsedTime;
    public GameState CurrentState { get; private set; }
    public event Action<GameState> OnGameStateChanged;
    public static GameManager Instance { get; private set; }
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetState(GameState newState)
    {
        CurrentState = newState;
        OnGameStateChanged?.Invoke(newState);

        switch (newState)
        {
            case GameState.MainMenu:
                HandleMainMenu();
                break;

            case GameState.Playing:
                HandlePlaying();
                break;
            
            case GameState.Paused:
                HandlePaused();
                break;

            case GameState.Lose:
                HandleLose();
                break;

            case GameState.Win:
                HandleWin();
                break;
        }
    }

    void Start()
    {
        SetState(GameState.MainMenu);
    }

    void Update()
    {
        if (CurrentState != GameState.Playing) return;

        _elapsedTime += Time.deltaTime;
        UpdateTimerUI();
    }

    public void StartGame(BoardConfigSO config)
    {
        _config = config;
        SetState(GameState.Playing);
        _camera.orthographicSize = _config.CameraSize;
        
        RectTransform rect = _hudEdgeL.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(_config.HUDEdgesSize, rect.sizeDelta.y);
        rect = _hudEdgeR.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(_config.HUDEdgesSize, rect.sizeDelta.y);
        
        _boardController.CreateBlankBoard(config.Width, config.Height);
        _totalTiles = config.Width * config.Height;
        //Espera primeiro click
        _elapsedTime = 0f;
        UpdateTimerUI();
    }

    void HandleMainMenu()
    {
        _mainMenu.SetActive(true);
        _hud.SetActive(false);
        _pauseMenu.SetActive(false);
        _gameOverMenu.SetActive(false);
    }

    void HandlePlaying()
    {
        _mainMenu.SetActive(false);
        _hud.SetActive(true);
        _pauseMenu.SetActive(false);
    }

    void HandlePaused()
    {
        _pauseMenu.SetActive(true);  
    }

    void HandleLose()
    {
        _gameOverMenu.SetActive(true);
        _gameOverMsg.text = "DERROTA!";
    }

    void HandleWin()
    {
        _gameOverMenu.SetActive(true);
        _gameOverMsg.text = "VITÓRIA!";
    }

    public void OnTileClicked(int x, int y, TileView tileView, bool isRightClick)
    {
        if (_isFirstClick)
        {
            if(isRightClick) return;
            _isFirstClick = false;

            FirstClick(x, y);
        }
        else
        {
            if (isRightClick)
            {
                _boardController.ChangeToFlag(tileView);
                return;
            }
        }
        tileView.Click();
    }

    public void RemoveFlag(TileView tileView)
    {
        _boardController.RemoveFlag(tileView);
    }

    void FirstClick(int x, int y)
    {
        _boardController.GenerateBoard(x, y, _config);
    }

    public void BombClicked()
    {
        SetState(GameState.Lose);
    }

    public void RestartGame()
    {
        Destroy(GameManager.Instance.gameObject);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void CheckForVictory(int revealedTiles)
    {
        if(revealedTiles + _config.MineCount == _totalTiles) SetState(GameState.Win);
    }

    public void Pause()
    {
        SetState(GameState.Paused);
    }

    public void Unpause()
    {
        SetState(GameState.Playing);
    }

    public void Hide()
    {
        
    }

    void UpdateTimerUI()
    {
        int totalSeconds = Mathf.FloorToInt(_elapsedTime);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        _timerText.text = $"{minutes:00}:{seconds:00}";
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

}
