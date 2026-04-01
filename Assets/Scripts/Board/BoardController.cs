using System.Collections;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    //Recebe clique
    //Manda gerar board
    //Atualiza Model
    //Atualiza View
    [SerializeField] BoardView _boardView;
    TileModel[,] _boardTileModel;
    GameObject[,] _boardObject;
    int[,] _boardRep;
    int _width;
    int _height;
    int _mineTracker;
    int _mineCount;
    bool _firstTileRevealedOnGameOver = true;

    public void CreateBlankBoard(int width, int height)
    {
        _width = width;
        _height = height;
        _boardTileModel = new TileModel[_width, _height];
        _boardView.CreateBlankBoard(_boardTileModel);
        _mineTracker = 0;
    }

    public void GenerateBoard(int x, int y, BoardConfigSO config)
    {
        BoardGenerator generator = new BoardGenerator();

        var mode = GameModeSelectorUI.GetSavedMode();
        bool useSolver = mode == GameMode.NoLuck;

        BoardSolver solver = useSolver ? new BoardSolver() : null;

        _boardRep = null;

        int attemptsUsed = 0;

        if (!useSolver)
        {
            _boardRep = generator.GenerateBoard(x, y, config);
            attemptsUsed = 1;
        }
        else
        {
            int maxAttempts = 100;
            bool solved = false;

            while (maxAttempts > 0 && !solved)
            {
                _boardRep = generator.GenerateBoard(x, y, config);
                solved = solver.IsSolvableWithoutGuess(_boardRep, x, y);
                maxAttempts--;
                attemptsUsed++;
            }

            Debug.Log($"Board gerado em {attemptsUsed} tentativa(s). Solved = {solved}");

            if (!solved)
            {
                Debug.LogWarning("Não foi possível gerar board resolvível dentro do limite de tentativas.");
            }
        }

        _boardObject = _boardView.UpdateBoardVisual(_boardRep);
        _mineCount = config.MineCount;

        SubscribeToTilesEvents();
    }

    public void ChangeToFlag(TileView tileView)
    {
        _boardView.ChangeToFlag(tileView);
        _mineTracker++;
        GameManager.Instance.UpdateMinesMarked(_mineTracker);
        BoardStatusUpdate();
        TryStartGameOverAnimation();
    }

    public void RemoveFlag(TileView tileView)
    {
        _boardView.RemoveFlag(tileView);
        _mineTracker--;
        GameManager.Instance.UpdateMinesMarked(_mineTracker);
    }

    void SubscribeToTilesEvents()
    {
        {
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    TileView tileView = _boardObject[x, y].GetComponent<TileView>();

                    if (tileView == null) continue;

                    tileView.OnTileClicked -= HandleOnTileClicked;
                    tileView.OnTileClicked += HandleOnTileClicked;
                    tileView.OnRevealFinished -= HandleOnRevealFinished;
                    tileView.OnRevealFinished += HandleOnRevealFinished;
                }
                
            }
        }
    }

    void HandleOnRevealFinished(TileView tileView)
    {
        TryStartGameOverAnimation();
    }

    void TryStartGameOverAnimation()
    {
        if (!GameManager.Instance.GameIsOver) return;
        if (!_firstTileRevealedOnGameOver) return;

        _firstTileRevealedOnGameOver = false;

        bool won = GameManager.Instance.CurrentState == GameState.Win;
        StartCoroutine(GameOverAnimation(won));
    }

    public void WinAnimationQA() //MÉTODO APENAS PARA QA
    {
        StartCoroutine(GameOverAnimation(true));
    }

    public void LoseAnimationQA() //MÉTODO APENAS PARA QA
    {
        StartCoroutine(GameOverAnimation(false));
    }

    IEnumerator GameOverAnimation(bool won)
    {
        if(won)
        {
            yield return StartCoroutine(_boardView.ChangeToDefused(_mineCount));
        }
        else yield return new WaitForSeconds(0.3f);
        yield return new WaitForSeconds(0.6f);
        GameManager.Instance.GameOver();
    }

    void HandleOnTileClicked(TileView tileView)
    {
        int x = tileView.X;
        int y = tileView.Y;

        if(tileView.IsMine) GameManager.Instance.MineClicked(tileView);
        else if(tileView.IsBlank) BlankTileClicked(tileView);
        else BoardStatusUpdate();
    }

    void BlankTileClicked(TileView tileView)
    {
        int x = tileView.X;
        int y = tileView.Y;
        for(int i = x - 1; i <= x + 1; i++)
        {
            for(int j = y - 1; j <= y + 1; j++)
            {
                if (i >= 0 && i < _width && j >= 0 && j < _height)
                {
                    _boardObject[i, j].GetComponent<TileView>().OnClick();
                }
            }
        }
        BoardStatusUpdate();
    }

    void BoardStatusUpdate()
    {
        int revealedTiles = 0;
        for(int i = 0; i < _width; i++)
        {
            for(int j = 0; j < _height; j++)
            {
                if(_boardObject[i, j].GetComponent<TileView>().IsRevealed) revealedTiles++;
            }
        }
        GameManager.Instance.CheckForVictory(revealedTiles);
    }

    public void PlayLoseFX(TileView tileView)
    {
        _boardView.SpawnMineExplosionFX(tileView);
    }

    public void ResetBoard()
    {
        if (_boardRep == null)
        {
            Debug.LogWarning("Não existe board salvo para repetir.");
            return;
        }

        _mineTracker = 0;
        _firstTileRevealedOnGameOver = true;

        _boardObject = _boardView.RebuildBoard(_boardRep);
        SubscribeToTilesEvents();
    }

    public void RevealTileAt(int x, int y)
    {
        if (_boardObject == null) return;
        if (x < 0 || x >= _width || y < 0 || y >= _height) return;

        var tileView = _boardObject[x, y].GetComponent<TileView>();
        if (tileView == null) return;

        //tenho q usar Click() pq OnClick() passa pelo GameManager
        tileView.Click();
    }
    
}
