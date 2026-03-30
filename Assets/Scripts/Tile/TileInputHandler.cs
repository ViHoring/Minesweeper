using UnityEngine;
using UnityEngine.EventSystems;

public class TileInputHandler : MonoBehaviour
{
    //Detecta clique do jogador
    
    Camera _camera;
    TileView _currentTile;

    void Start()
    {
        _camera = Camera.main;
    }

    void Update()
    {
        if (GameManager.Instance.CurrentState != GameState.Playing)
            return;

        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            TileView tile = hit.collider.GetComponent<TileView>();

            if (tile != _currentTile)
            {
                if (_currentTile != null)
                    _currentTile.OnHoverExit();

                _currentTile = tile;

                if (_currentTile != null)
                    _currentTile.OnHoverEnter();
            }

            if (tile != null && Input.GetMouseButtonDown(0))
            {
                tile.OnClick();
            }

            if (tile != null && Input.GetMouseButtonDown(1))
            {
                tile.OnRightClick();
            }
        }
        else
        {
            if (_currentTile != null)
            {
                _currentTile.OnHoverExit();
                _currentTile = null;
            }
        }
    }
}
