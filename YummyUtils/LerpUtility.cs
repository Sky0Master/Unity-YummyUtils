using System;
using System.Collections;
using UnityEngine;

namespace VinoUtility{
public static class LerpUtility
{
    public static IEnumerator Lerp(float start, float end, float duration, Action<float> callback, Action onComplete = null)
    {
        float t = 0f;
        float time = 0f;
        while (t <= 1f)
        {
            callback?.Invoke(Mathf.Lerp(start, end, t));
            time += Time.unscaledDeltaTime;
            t = time / duration;
            yield return null;
        }

        callback?.Invoke(end);
        onComplete?.Invoke();
    }

    public static IEnumerator Lerp(Vector3 start, Vector3 end, float duration, Action<Vector3> callback, Action onComplete = null)
    {
        float t = 0f;
        float time = 0f;
        while (t <= 1f)
        {
            callback?.Invoke(Vector3.Lerp(start, end, t));
            time += Time.unscaledDeltaTime;
            t = time / duration;
            yield return null;
        }

        callback?.Invoke(end);
        onComplete?.Invoke();
    }

    public static IEnumerator Lerp(Vector2 start, Vector2 end, float duration, Action<Vector2> callback, Action onComplete = null)
    {
        float t = 0f;
        float time = 0f;
        while (t <= 1f)
        {
            callback?.Invoke(Vector2.Lerp(start, end, t));
            time += Time.unscaledDeltaTime;
            t = time / duration;
            yield return null;
        }
        callback?.Invoke(end);
        onComplete?.Invoke();
    }
    public static IEnumerator LerpPhysics(Vector2 start, Vector2 end, float duration, Action<Vector2> callback, Action onComplete = null)
    {
        float t = 0f;
        float time = 0f;
        while (t <= 1f)
        {
            callback?.Invoke(Vector2.Lerp(start, end, t));
            time += Time.fixedDeltaTime;
            t = time / duration;
            yield return new WaitForFixedUpdate();
        }
        callback?.Invoke(end);
        onComplete?.Invoke();
    }
}
}