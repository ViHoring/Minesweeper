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

    public void CreateBlankBoard(int width, int height)
    {
        _width = width;
        _height = height;
        _board = new TileModel[_width, _height];
        _boardView.CreateBlankBoard(_board);
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
    }

    public void RemoveFlag(TileView tileView)
    {
        _boardView.RemoveFlag(tileView);
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

                    tileView.OnRevealFinished -= HandleTileRevealFinished;
                    tileView.OnRevealFinished += HandleTileRevealFinished;
                }
                
            }
        }
    }

    void HandleTileRevealFinished(TileView tileView)
    {
        int x = tileView.X;
        int y = tileView.Y;

        if(tileView.IsBomb) GameManager.Instance.BombClicked();
        if(tileView.IsBlank) BlankTileClicked(tileView);
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
    }
}
