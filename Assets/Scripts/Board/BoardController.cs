using System.Collections;
using UnityEngine;

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
    int _bombTracker;
    bool _gameFinished;

    public void CreateBlankBoard(int width, int height)
    {
        _width = width;
        _height = height;
        _board = new TileModel[_width, _height];
        _boardView.CreateBlankBoard(_board);
        _bombTracker = 0;
    }

    public void GenerateBoard(int x, int y, BoardConfigSO config)
    {
        BoardGenerator generator = new BoardGenerator();
        int[,] boardRep = generator.GenerateBoard(x, y, config);
        _boardObject = _boardView.UpdateBoardVisual(boardRep);

        SubscribeToTilesEvents();
    }

    public void ChangeToFlag(TileView tileView)
    {
        _boardView.ChangeToFlag(tileView);
        _bombTracker++;
        GameManager.Instance.UpdateBumbsMarked(_bombTracker);
    }

    public void RemoveFlag(TileView tileView)
    {
        _boardView.RemoveFlag(tileView);
        _bombTracker--;
        GameManager.Instance.UpdateBumbsMarked(_bombTracker);
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
        if(GameManager.Instance.CurrentState == GameState.Win || GameManager.Instance.CurrentState == GameState.Lose)
        {
            StartCoroutine(StartBoardAnimation());
        }
    }

    IEnumerator StartBoardAnimation()
    {
        yield return new WaitForSeconds(0.6f);
        GameManager.Instance.GameOver();
    }

    void HandleOnTileClicked(TileView tileView)
    {
        int x = tileView.X;
        int y = tileView.Y;

        if(tileView.IsBomb) GameManager.Instance.BombClicked();
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
