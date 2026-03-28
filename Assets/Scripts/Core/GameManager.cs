using System;
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
    [SerializeField] Camera _camera;
    [SerializeField] BoardController _boardController;
    bool _isFirstClick = true;
    BoardConfigSO _config;
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

    public void StartGame(BoardConfigSO config)
    {
        _config = config;
        SetState(GameState.Playing);
        float size = config.Height * 0.5f + 0.75f;
        _camera.orthographicSize = size;
        _boardController.CreateBlankBoard(config.Width, config.Height);
        //Espera primeiro click
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
        //voltar contagem de tempo   
    }

    void HandlePaused()
    {
        _pauseMenu.SetActive(true);
        //parar contagem de tempo            
    }

    void HandleLose()
    {
        _gameOverMenu.SetActive(true);
    }

    void HandleWin()
    {
        
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

}
