using System.Collections;
using UnityEngine;
using System;

public static class MonoExt
{
    public static void WaitForOneFrame(this MonoBehaviour caller, System.Action callback)
    {
        caller.StartCoroutine(DelayOneFrameCoroutine(callback));
    }

    public static Coroutine WaitForTime(this MonoBehaviour self, float seconds, Action action)
    {
        return self.StartCoroutine(DelayTimeCoroutine(seconds, action));
    }

    public static void Log(this MonoBehaviour self, object message)
    {
        Debug.Log($"[{self.GetType().Name}] {message}", self);
    }

    #region Private Methods
    private static IEnumerator DelayOneFrameCoroutine(Action callback)
    {
        yield return null;
        callback?.Invoke();
    }
    private static IEnumerator DelayTimeCoroutine(float seconds, Action action)
    {
        yield return new WaitForSeconds(seconds);
        action?.Invoke();
    }
    #endregion
}
