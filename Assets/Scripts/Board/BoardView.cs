using UnityEngine;

public class BoardView : MonoBehaviour
{
    //Instancia tiles
    //Atualiza visual do board
    
    [SerializeField] GameObject _tilePrefab1;
    [SerializeField] GameObject _tilePrefab2;
    [SerializeField] GameObject _tilePrefab3;
    [SerializeField] GameObject _tilePrefab4;
    [SerializeField] GameObject _tilePrefab5;
    [SerializeField] GameObject _tilePrefab6;
    [SerializeField] GameObject _tilePrefab7;
    [SerializeField] GameObject _tilePrefab8;
    [SerializeField] GameObject _tilePrefabBlank;
    [SerializeField] GameObject _tilePrefabBomb;
    [SerializeField] GameObject _tilePrefabFlag;
    GameObject[,] _tiles;
    int _width;
    int _height;
    float _offsetX;
    float _offsetY;

    public void CreateBlankBoard(TileModel[,] board)
    {
        _width = board.GetLength(0);
        _height = board.GetLength(1);

        _offsetX = _width / 2f;
        _offsetY = _height / 2f;

        _tiles = new GameObject[_width, _height];

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {         
                _tiles[x, y] = InstantiatePrefab(x, y, _tilePrefabBlank);
            }
        }
    }

    GameObject InstantiatePrefab(int x, int y, GameObject prefab)
    {

        Vector3 position = new Vector3(
                    x - _offsetX,
                    0,
                    y - _offsetY
        );

        GameObject tile = Instantiate(
            prefab,        
            position,
            Quaternion.Euler(0, 180, 180),
            transform
        );

        TileView tileView = tile.GetComponent<TileView>();
        if (tileView != null)
        {
            tileView.Init(x, y);
        }        
        return tile;
    }

    public void UpdateBoardVisual(int[,] boardRep)
    {
        //Percorre a matriz boardRep, se -1 é bomba, 0 é blank, número maior que zero é número correspondente:
        for(int x = 0; x < _width; x++)
        {
            for(int y = 0; y < _height; y++)
            {
                //Se não for vazio:
                if(boardRep[x, y] != 0)
                {
                    Destroy(_tiles[x, y]);
                    GameObject prefab = GetPrefab(boardRep[x, y]);

                    _tiles[x, y] = InstantiatePrefab(x, y, prefab);
                }
            }
        }
    }

    GameObject GetPrefab(int value)
    {
        if (value == -1) return _tilePrefabBomb;
        if (value == 0) return _tilePrefabBlank;

        switch (value)
        {
            case 1: return _tilePrefab1;
            case 2: return _tilePrefab2;
            case 3: return _tilePrefab3;
            case 4: return _tilePrefab4;
            case 5: return _tilePrefab5;
            case 6: return _tilePrefab6;
            case 7: return _tilePrefab7;
            case 8: return _tilePrefab8;
            default: return _tilePrefabBlank;
        }
    }
}
