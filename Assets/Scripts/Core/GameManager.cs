using System;
using Unity.VisualScripting;
using UnityEngine;

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
        Test();
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
        //_camera.transform.position = new Vector3(0, config.Height, -config.Height);
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
        
    }

    void HandleWin()
    {
        
    }

    public void OnTileClicked(int x, int y, TileView tileView)
    {
        if (_isFirstClick)
        {
            _isFirstClick = false;

            FirstClick(x, y);
        }
        else
        {
            tileView.Click();
        }
    }

    void FirstClick(int x, int y)
    {
        _boardController.GenerateBoard(x, y, _config);
    }


    void Test()
    {
        int[,] test = new int[3,3];
        for(int i = 0; i < 3;  i++)
        {
            for(int j = 0; j < 3;  j++)
            {
                Debug.Log(test[i, j]);
            }
        }
    }
}
