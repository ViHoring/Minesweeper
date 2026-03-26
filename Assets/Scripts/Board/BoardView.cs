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

    public void CreateBlankBoard(TileModel[,] board)
    {
        int width = board.GetLength(0);
        int height = board.GetLength(1);

        _tiles = new GameObject[width, height];

        float offsetX = width / 2f;
        float offsetY = height / 2f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 position = new Vector3(
                    x - offsetX,
                    0,
                    y - offsetY
                );

                GameObject tile = Instantiate(
                    _tilePrefabBlank,
                    position,
                    Quaternion.Euler(0, 0, 180),
                    transform
                );

                _tiles[x, y] = tile;
            }
        }
    }
}
