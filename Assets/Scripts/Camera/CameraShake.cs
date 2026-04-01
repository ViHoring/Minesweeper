using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
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
}