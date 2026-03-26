using UnityEngine;

public class BoardController : MonoBehaviour
{
    //Recebe clique
    //Manda gerar board
    //Atualiza Model
    //Atualiza View
    [SerializeField] BoardView _boardView;
    TileModel[,] _board;
    int _width;
    int _height;

    public void CreateBlankBoard(int width, int height)
    {
        _width = width;
        _height = height;
        TileModel[,] _board = new TileModel[_width, _height];
        _boardView.CreateBlankBoard(_board);
    }

    
    public void GenerateBoard(int x, int y)
    {
        
    }

    //Converte Indice para matriz
    /*int index = positions[i];
    int x = index % width;
    int y = index / width;
    board[x, y].HasMine = true;*/


}
