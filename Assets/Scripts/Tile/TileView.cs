using System;
using System.Collections;
using UnityEngine;

public class TileView : MonoBehaviour
{
    //Representação visual do tile
    //Controla animações
    
    public event Action<TileView> OnTileClicked;
    public event Action<TileView> OnRevealFinished;
    Vector3 _originalScale;
    Renderer _renderer;
    Color _originalColor;
    int _x;
    int _y;
    public int X => _x;
    public int Y => _y;
    bool _isRevealed; public bool IsRevealed => _isRevealed;
    bool _isAnimating;
    bool _isFlaged; public bool IsFlaged => _isFlaged;
    bool _isFlag; public bool IsFlag => _isFlag;
    bool _isMine; public bool IsMine => _isMine;
    bool _isBlank; public bool IsBlank => _isBlank;

    void Awake()
    {
        _originalScale = transform.localScale;

        _renderer = GetComponentInChildren<Renderer>();
        if (_renderer != null) _originalColor = _renderer.material.color;
    }

    public void Init(int x, int y, bool isFlag, bool isMine, bool isBlank)
    {
        _x = x;
        _y = y;
        _isFlag = isFlag;
        _isMine = isMine;
        _isBlank = isBlank;
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
        if(_isRevealed || _isAnimating || _isFlaged || _isFlag) return;
        GameManager.Instance.OnTileClicked(_x, _y, this, false);
    }

    public void OnRightClick()
    {
        if (_isRevealed || _isAnimating) return;

        // Se este objeto é a própria flag overlay, remover a flag do tile base
        if (_isFlag)
        {
            GameManager.Instance.RemoveFlag(this);
            return;
        }

        OnHoverExit();

        if (_isFlaged)
        {
            GameManager.Instance.RemoveFlag(this);
            _isFlaged = false;
            return;
        }

        GameManager.Instance.OnTileClicked(_x, _y, this, true);
        _isFlaged = true;
    }

    void Highlight(bool isHovering)
    {
        if (_isRevealed || _isAnimating || _isFlaged || _isFlag) return;

        StopAllCoroutines();

        if (isHovering)
        {
            StartCoroutine(ScaleAnimation(_originalScale * 1.1f, 0.25f));
            _renderer.material.color = _originalColor * 1.2f;
        }
        else
        {
            StartCoroutine(ScaleAnimation(_originalScale, 0.25f));
            _renderer.material.color = _originalColor;
        }
    }

    IEnumerator ScaleAnimation(Vector3 targetScale, float duration)
    {
        Vector3 startScale = transform.localScale;
        float time = 0f;

        while (time < duration)
        {
            transform.localScale = Vector3.Lerp(startScale, targetScale, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
    }

    public void Click()
    {
        if (_isAnimating || _isRevealed) return;
        _isRevealed = true;
        OnTileClicked?.Invoke(this);
        StartCoroutine(ClickRoutine());
    }

    IEnumerator ClickRoutine()
    {
        _isAnimating = true;

        yield return StartCoroutine(ClickAnimation());
        yield return StartCoroutine(FlipAnimation());

        
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


        //Fase 2: vira
        Quaternion originalRotation = transform.localRotation;
        transform.localRotation = originalRotation * Quaternion.Euler(0, 0, 180);

        //Fase 3: abre
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
        OnRevealFinished?.Invoke(this);
    }

    public void SetFlagged(bool flagged)
    {
        _isFlaged = flagged;
    }

}
