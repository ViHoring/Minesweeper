using System.Collections;
using UnityEngine;

public class TileView : MonoBehaviour
{
    //Representação visual do tile
    //Controla animações

    Vector3 _originalScale;
    Renderer _renderer;
    Color _originalColor;
    int _x;
    int _y;
    bool _isFliped;
    bool _isAnimating;

    void Start()
    {
        _originalScale = transform.localScale;

        _renderer = GetComponentInChildren<Renderer>();
        _originalColor = _renderer.material.color;
    }

    public void Init(int x, int y)
    {
        _x = x;
        _y = y;
    }

    public void OnHoverEnter()
    {
        Highlight(true);
    }

    public void OnHoverExit()
    {
        Highlight(false);
    }

    public void OnClick()
    {
        if(_isFliped || _isAnimating) return;
        GameManager.Instance.OnTileClicked(_x, _y, this, false);
    }

    public void OnRightClick()
    {
        if(_isFliped || _isAnimating) return;
        GameManager.Instance.OnTileClicked(_x, _y, this, true);
    }

    void Highlight(bool isHovering)
    {
        if(_isFliped || _isAnimating) return;
        if (isHovering)
        {
            transform.localScale = _originalScale * 1.1f;
            _renderer.material.color = _originalColor * 1.2f;
        }
        else
        {
            transform.localScale = _originalScale;
            _renderer.material.color = _originalColor;
        }
    }

    public void Click()
    {
        StopAllCoroutines();
        _isAnimating = true;
        StartCoroutine(ClickAnimation());
        StartCoroutine(FlipAnimation());
        _isFliped = true;
        _isAnimating = false;
    }

    IEnumerator ClickAnimation()
    {
        Vector3 downScale = _originalScale * 0.85f;
        float duration = 0.08f;
        float time = 0;

        // aperta
        while (time < duration)
        {
            transform.localScale = Vector3.Lerp(_originalScale, downScale, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        // volta
        time = 0;
        while (time < duration)
        {
            transform.localScale = Vector3.Lerp(downScale, _originalScale, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        transform.localScale = _originalScale;
    }

    IEnumerator FlipAnimation()
    {
        float duration = 0.15f;
        float time = 0;

        Vector3 originalScale = transform.localScale;

        //Fase 1: fecha
        while (time < duration)
        {
            float t = time / duration;
            transform.localScale = new Vector3(
                Mathf.Lerp(originalScale.y, 0f, t),
                originalScale.y,
                originalScale.z
            );
            time += Time.deltaTime;
            yield return null;
        }

        transform.localScale = new Vector3(0, originalScale.y, originalScale.z);

        //Fase  2: Se flag, troca o tile
        //if(isRightClick) SwapVisual();

        //Fase 3: vira
        Quaternion originalRotation = transform.localRotation;
        transform.localRotation = originalRotation * Quaternion.Euler(0, 0, 180);

        //Fase 2: abre
        time = 0;
        while (time < duration)
        {
            float t = time / duration;
            transform.localScale = new Vector3(
                Mathf.Lerp(0f, originalScale.y, t),
                originalScale.y,
                originalScale.z
            );

            time += Time.deltaTime;
            yield return null;
        }

        transform.localScale = originalScale;
    }

    /*void SwapVisual()
    {
        // Exemplo:
        // destruir filho atual e instanciar outro

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        GameObject newVisual = Instantiate(_tilePrefabFlag, transform);
        newVisual.transform.localPosition = Vector3.zero;
    }*/
}
