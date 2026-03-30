using System;
using System.Collections;
using System.Numerics;
using TMPro;
using Unity.VectorGraphics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

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
    [SerializeField] TMP_Text _timerTextHUD;
    [SerializeField] TMP_Text _timerTextFinale;
    [SerializeField] GameObject _timerFinale;
    [SerializeField] TMP_Text _difficultyText;
    [SerializeField] GameObject _difficultyObject;
    [SerializeField] GameObject _pauseButton;
    [SerializeField] GameObject _showGameOverButton;
    [SerializeField] TMP_Text _minesMarkedTracker;
    [SerializeField] TMP_Text _minesTotalNumber;
    [SerializeField] HUDAnimationHandler _hudAnimatorHandler;
    bool _gameIsOver; public bool GameIsOver => _gameIsOver;
    bool _isFirstClick = true;
    BoardConfigSO _config;
    int _totalTiles;
    float _elapsedTime;
    bool _chillMode;
    int _minesMarked;
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

            case GameState.GameOver:
                HandleGameOver();
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
        
        Transform transform = _camera.GetComponent<Transform>();
        transform.localPosition = new UnityEngine.Vector3(transform.localPosition.x, _config.CameraPosition, transform.localPosition.z);
        
        _boardController.CreateBlankBoard(config.Width, config.Height);
        _totalTiles = config.Width * config.Height;
        _difficultyText.text = _config.DifficultyText;
        _minesTotalNumber.text = _config.MineCount.ToString();
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

    void HandleGameOver()
    {
        _gameOverMenu.SetActive(true);
    }

    void HandleLose()
    {
        SetGameOverScreen();
        _gameOverMsg.text = "DERROTA!";
    }

    void HandleWin()
    {
        SetGameOverScreen();
        _gameOverMsg.text = "VITÓRIA!";
    }

    void SetGameOverScreen()
    {
        _gameIsOver = true;
        _timerTextFinale.text = _timerTextHUD.text;
        if(_chillMode)
        {
            _timerFinale.SetActive(false);
            UnityEngine.Vector3 pos = _difficultyObject.transform.position;
            pos.y = 45.5f;
            _difficultyObject.transform.position = pos;
        }
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

    public void MineClicked()
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
        if(revealedTiles + _config.MineCount == _totalTiles && _minesMarked == _config.MineCount) SetState(GameState.Win);
    }

    public void Pause()
    {
        if(CurrentState == GameState.Playing) SetState(GameState.Paused);
    }

    public void Unpause()
    {
        if(CurrentState == GameState.Paused) SetState(GameState.Playing);
    }

    public void GameOver()
    {
        SetState(GameState.GameOver);
    }

    void UpdateTimerUI()
    {
        int totalSeconds = Mathf.FloorToInt(_elapsedTime);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        _timerTextHUD.text = $"{minutes:00}:{seconds:00}";
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void HideGameOverScreen()
    {
        _gameOverMenu.SetActive(false);
        _pauseButton.SetActive(false);
        _showGameOverButton.SetActive(true);
    }

    public void ShowGameOverScreen()
    {
        _gameOverMenu.SetActive(true);
        _pauseButton.SetActive(true);
        _showGameOverButton.SetActive(false);
    }

    public void UpdateBumbsMarked(int minesMarked)
    {
        _minesMarked = minesMarked;
        _minesMarkedTracker.text = minesMarked.ToString();
    }

    public void ChillToggle()
    {
        _chillMode = !_chillMode;
    }

    public bool GetChillModeInfo()
    {
        return _chillMode;
    }

}
