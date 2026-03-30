using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardController : MonoBehaviour
{
    //Recebe clique
    //Manda gerar board
    //Atualiza Model
    //Atualiza View
    [SerializeField] BoardView _boardView;
    TileModel[,] _board;
    GameObject[,] _boardObject;
    int _width;
    int _height;
    int _mineTracker;
    int _mineCount;
    bool _firstTileRevealedOnGameOver = true;

    public void CreateBlankBoard(int width, int height)
    {
        _width = width;
        _height = height;
        _board = new TileModel[_width, _height];
        _boardView.CreateBlankBoard(_board);
        _mineTracker = 0;
    }

    public void GenerateBoard(int x, int y, BoardConfigSO config)
    {
        BoardGenerator generator = new BoardGenerator();
        int[,] boardRep = generator.GenerateBoard(x, y, config);
        _boardObject = _boardView.UpdateBoardVisual(boardRep);
        _mineCount  = config.MineCount;

        SubscribeToTilesEvents();
    }

    public void ChangeToFlag(TileView tileView)
    {
        _boardView.ChangeToFlag(tileView);
        _mineTracker++;
        GameManager.Instance.UpdateBumbsMarked(_mineTracker);
        BoardStatusUpdate();
    }

    public void RemoveFlag(TileView tileView)
    {
        _boardView.RemoveFlag(tileView);
        _mineTracker--;
        GameManager.Instance.UpdateBumbsMarked(_mineTracker);
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
        if(!GameManager.Instance.GameIsOver) return;
        else if(_firstTileRevealedOnGameOver)
        {
            bool won = false;
            if(GameManager.Instance.CurrentState == GameState.Win) won = true;
            StartCoroutine(GameOverAnimation(won));
        }
        _firstTileRevealedOnGameOver = false;
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
        if(!won)
        {
            for(int i = 0; i < _width; i++)
            {
                for(int j = 0; j < _height; j++)
                {
                    TileView tileView = _boardObject[i, j].GetComponent<TileView>();
                    if(!tileView.IsRevealed) 
                    {
                        tileView.OnClick();
                    }
                }
            }
        }
        else yield return StartCoroutine(_boardView.ChangeToDefused(_mineCount));
        yield return new WaitForSeconds(0.3f);
        GameManager.Instance.GameOver();
    }

    void HandleOnTileClicked(TileView tileView)
    {
        int x = tileView.X;
        int y = tileView.Y;

        if(tileView.IsMine) GameManager.Instance.MineClicked();
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
}
