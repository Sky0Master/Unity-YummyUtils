using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Yummy{

/// <summary>
/// Transform 扩展
/// </summary>
public static class TransformExt
{
    /// <summary>
    /// 获取指定子路径的物体上的组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="transform"></param>
    /// <param name="childPath"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static T GetComponent<T>(this Transform transform, string childPath){
        try{
            Transform child = transform.Find(childPath);
            if(child == null){
                throw new Exception(string.Format("[{0}] has no such child [{1}]", transform.gameObject.name, childPath));
            }
            return child.GetComponent<T>();
        } catch (NullReferenceException e) {
            Console.WriteLine(e.Message);
            throw new Exception(string.Format(
                "[{0}/{1}] has no such component [{2}]",
                transform.gameObject.name, childPath, typeof(T).Name
                ));
        }
    }
    
    /// <summary>
    /// 获取从当前Transform指向鼠标位置的2D向量
    /// </summary>
    /// <param name="invoker"></param>
    /// <returns></returns>
    public static Vector2 GetMouseVector2(this Transform invoker)
    {
        var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return (Vector2)pos - (Vector2)invoker.position;
    }

    public static void MakeChildrenInLine(this Transform invoker, float dist = 1f)
    {
        var pa = invoker;
        var children = GetActiveChildren(pa);
        for (int i = 0; i < children.Count; i++)
        {
            children[i].localPosition = new Vector3(dist * i, 0, 0);
        }
    }
    public static List<Transform> GetActiveChildren(this Transform node)
    {
        List<Transform> children = new List<Transform>();
        for(int i = 0; i < node.childCount; i++)
        {
            var child = node.GetChild(i);
            if (child.gameObject.activeSelf)
                children.Add(child);
        }
        return children;
    }
    public static List<Transform> GetInactiveChildren(this Transform node)
    {
        List<Transform> children = new List<Transform>();
        for (int i = 0; i < node.childCount; i++)
        {
            var child = node.GetChild(i);
            if (!child.gameObject.activeSelf)
                children.Add(child);
        }
        return children;
    }
    public static List<Transform> GetAllChildren(this Transform node)
    {
        List<Transform> children = new List<Transform>();
        for (int i = 0; i < node.childCount; i++)
        {
            children.Add(node.GetChild(i));
        }
        return children;
    } 

    public static void RenameChildrenByIndex(this Transform invoker, int startIndex = 0)
    {
        for (int i = 0; i < invoker.childCount; i++)
        {
            invoker.GetChild(i).name = (i + startIndex).ToString();
        }
    }

    public static IEnumerator PlayPositionAnimation(this Transform invoker, AnimationCurve xCurve, AnimationCurve yCurve, AnimationCurve zCurve,float duration = 1f, float factor = 1f)
    {
        float t = 0;
        var x = invoker.position.x;
        var y = invoker.position.y;
        var z = invoker.position.z;
        while (t < duration)
        {
            invoker.position = new Vector3(x + factor * xCurve.Evaluate(t / duration), y + factor * yCurve.Evaluate(t / duration), z + factor * zCurve.Evaluate(t / duration));
            t += Time.deltaTime;
            yield return null;
        }
    }
    public static IEnumerator PlayScaleAnimation(this Transform invoker, AnimationCurve scaleCurve, float duration)
    {
        
        float t = 0;
        var x = invoker.localScale.x;
        var y = invoker.localScale.y;
        var z = invoker.localScale.z;
        while (t < duration)
        {
            invoker.localScale = new Vector3(x * scaleCurve.Evaluate(t / duration),y * scaleCurve.Evaluate(t / duration), z * scaleCurve.Evaluate(t / duration));
            t += Time.deltaTime;
            yield return null;
        }
    }

    #region Editor Functions
    #if UNITY_EDITOR
    
    public static void DestroyAllChildrenEditor(this Transform invoker)
    {
        var pa = invoker;
        var children = GetAllChildren(pa);
        foreach (var child in children)
        {
           Undo.DestroyObjectImmediate(child.gameObject);
        }
    }

    #endif
    #endregion
}
}
