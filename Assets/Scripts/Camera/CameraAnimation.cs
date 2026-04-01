using System.Collections;
using UnityEngine;

public class CameraAnimation : MonoBehaviour
{
    [SerializeField] Camera _camera;
    Coroutine _zoomRoutine;
    Vector3 _originalLocalPosition;
    Coroutine _shakeRoutine;

    public void Shake(float duration, float strength)
    {
        _originalLocalPosition = transform.localPosition;

        if (_shakeRoutine != null)
            StopCoroutine(_shakeRoutine);

        _shakeRoutine = StartCoroutine(ShakeRoutine(duration, strength));
    }

    IEnumerator ShakeRoutine(float duration, float strength)
    {
        float time = 0f;

        while (time < duration)
        {
            Vector3 offset = new Vector3(
                Random.Range(-strength, strength),
                Random.Range(-strength, strength),
                0f
            );

            transform.localPosition = _originalLocalPosition + offset;

            time += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = _originalLocalPosition;
        _shakeRoutine = null;
    }

    public IEnumerator PlayPunchZoom()
    {
        if (_camera == null || !_camera.orthographic)
            yield break;

        if (_zoomRoutine != null)
            StopCoroutine(_zoomRoutine);

        yield return StartCoroutine(PunchZoomRoutine());
    }

    IEnumerator PunchZoomRoutine()
    {
        float originalSize = _camera.orthographicSize;
        float zoomedSize = originalSize - 0.35f;

        float inDuration = 0.08f;
        float outDuration = 0.10f;

        float time = 0f;

        // aproxima
        while (time < inDuration)
        {
            _camera.orthographicSize = Mathf.Lerp(originalSize, zoomedSize, time / inDuration);
            time += Time.deltaTime;
            yield return null;
        }
        _camera.orthographicSize = zoomedSize;

        // volta
        time = 0f;
        while (time < outDuration)
        {
            _camera.orthographicSize = Mathf.Lerp(zoomedSize, originalSize, time / outDuration);
            time += Time.deltaTime;
            yield return null;
        }

        _camera.orthographicSize = originalSize;
        _zoomRoutine = null;
    }
}