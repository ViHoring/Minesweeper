using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HUDAnimationHandler : MonoBehaviour
{
    //Controla a animação do HUD

    Vector3 _originalScale;
    [SerializeField] Button _hideHUDButton;

    void Awake()
    {
        _originalScale = transform.localScale;
        if(GameManager.Instance.GetChillModeInfo())
        {
            HideAnimation();
            Debug.Log("Chamou");
        }
         
    }
    public void HideAnimation()
    {
        StartCoroutine(HideHudAnimation());
    }

    public void ShowAnimation()
    {
        StartCoroutine(ShowHudAnimation());
    }
    
    IEnumerator HideHudAnimation()
    {
        Vector3 downScale = _originalScale * 0;
        float duration = 0.3f;
        float time = 0f;

        while (time < duration)
        {
            transform.localScale = Vector3.Lerp(_originalScale, downScale, time / duration);

            time += Time.deltaTime;
            yield return null;
        }

        transform.localScale = downScale;
    }

    IEnumerator ShowHudAnimation()
    {
        Vector3 downScale = _originalScale * 1;
        float duration = 0.3f;
        float time = 0f;

        while (time < duration)
        {
            transform.localScale = Vector3.Lerp(_originalScale, downScale, time / duration);

            time += Time.deltaTime;
            yield return null;
        }

        transform.localScale = downScale;
    }
}
