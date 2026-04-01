using System.Collections;
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
    [SerializeField] GameObject _tilePrefabMine;
    [SerializeField] GameObject _tilePrefabFlag;
    [SerializeField] GameObject _tilePrefabDefused;
    [SerializeField] GameObject _mineExplosionFXPrefab;
    [SerializeField] Transform _overlaysRoot;
    GameObject[,] _tiles;
    GameObject[,] _flags;
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
                _tiles[x, y] = InstantiatePrefab(x, y, _tilePrefabBlank, this.transform, false, false, true);
            }
        }
    }

    GameObject InstantiatePrefab(int x, int y, GameObject prefab, Transform parent, bool isFlag, bool isMine, bool isBlank)
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
            parent
        );

        TileView tileView = tile.GetComponent<TileView>();
        if (tileView != null)
        {
            tileView.Init(x, y, isFlag, isMine, isBlank);
        }

        return tile;
    }

    public GameObject[,] UpdateBoardVisual(int[,] boardRep)
    {
        bool isMine = false;
        //Percorre a matriz boardRep, se -1 é mina, 0 é blank, número maior que zero é número correspondente:
        for(int x = 0; x < _width; x++)
        {
            for(int y = 0; y < _height; y++)
            {
                //Se não for vazio:
                if(boardRep[x, y] != 0)
                {
                    isMine = false;
                    Destroy(_tiles[x, y]);
                    GameObject prefab = GetPrefab(boardRep[x, y]);

                    if(boardRep[x, y] == -1) isMine = true;

                    _tiles[x, y] = InstantiatePrefab(x, y, prefab, this.transform, false, isMine, false);
                }
            }
        }
        return _tiles;
    }

    GameObject GetPrefab(int value)
    {
        if (value == -1) return _tilePrefabMine;
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

    public void ChangeToFlag(TileView tileView)
    {
        int x = tileView.X;
        int y = tileView.Y;

        if (_flags == null) _flags = new GameObject[_width, _height];

        if (_flags[x, y] != null) return;

        _flags[x, y] = InstantiatePrefab(x, y, _tilePrefabFlag, _overlaysRoot, true, false, false);

        TileView baseTileView = _tiles[x, y].GetComponent<TileView>();
        if (baseTileView != null)
        {
            baseTileView.SetFlagged(true);
        }
    }

    public void RemoveFlag(TileView tileView)
    {
        int x = tileView.X;
        int y = tileView.Y;

        if (_flags != null && _flags[x, y] != null)
        {
            Destroy(_flags[x, y]);
            _flags[x, y] = null;
        }

        TileView baseTileView = _tiles[x, y].GetComponent<TileView>();
        if (baseTileView != null)
        {
            baseTileView.SetFlagged(false);
        }
    }

    public IEnumerator ChangeToDefused(int mines)
    {
        float duration = 3f / mines;
        for(int i = 0; i < _width; i++)
        {
            for(int j = 0; j < _height; j++)
            {
                if(_flags[i, j] != null) Destroy(_flags[i, j]);
                if(_tiles[i, j].GetComponent<TileView>().IsMine == true)
                {
                    InstantiatePrefab(i, j, _tilePrefabDefused, _overlaysRoot, false, false, false);
                    yield return new WaitForSeconds(duration);
                }
            }
        }
    }

    public void SpawnMineExplosionFX(TileView tileView)
    {
        if (_mineExplosionFXPrefab == null) return;

        int x = tileView.X;
        int y = tileView.Y;

        Vector3 position = new Vector3(
            x - _offsetX,
            0.1f,
            y - _offsetY
        );

        Instantiate(_mineExplosionFXPrefab, position, Quaternion.identity);
    }

    public GameObject[,] RebuildBoard(int[,] boardRep)
    {
        ClearBoardVisuals();

        _width = boardRep.GetLength(0);
        _height = boardRep.GetLength(1);

        _offsetX = _width / 2f;
        _offsetY = _height / 2f;

        _tiles = new GameObject[_width, _height];
        _flags = new GameObject[_width, _height];

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                bool isMine = boardRep[x, y] == -1;
                bool isBlank = boardRep[x, y] == 0;

                GameObject prefab = GetPrefab(boardRep[x, y]);
                _tiles[x, y] = InstantiatePrefab(x, y, prefab, this.transform, false, isMine, isBlank);
            }
        }

        return _tiles;
    }

    void ClearBoardVisuals()
    {
        if (_tiles != null)
        {
            for (int x = 0; x < _tiles.GetLength(0); x++)
            {
                for (int y = 0; y < _tiles.GetLength(1); y++)
                {
                    if (_tiles[x, y] != null)
                        Destroy(_tiles[x, y]);
                }
            }
        }
        _flags = null;
        ClearOverlays();
    }

    void ClearOverlays()
    {
        if (_overlaysRoot == null) return;

        for (int i = _overlaysRoot.childCount - 1; i >= 0; i--)
        {
            Destroy(_overlaysRoot.GetChild(i).gameObject);
        }
    }
}
