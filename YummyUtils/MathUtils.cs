using System.Collections.Generic;
using UnityEngine;

namespace Yummy{
public static class MathUtils
{
    /// <summary>
    /// 列表里随机抽取 count 个不重复的元素, count必须小于列表长度
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="count"></param>
    /// <param name="arr"></param>
    /// <returns></returns>
    public static List<T> GetRandomElementsNonRepeat<T>(int count, List<T> arr)
    {
        if(count == arr.Count)
        {
            return arr;
        }
        List<T> result = new List<T>();
        List<T> temp = new List<T>(arr); 
        var len = count;
        // 随机选择 count 个不重复的元素
        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0,len);
            result.Add(temp[index]);
            temp.RemoveAt(index);
        }
        return result;
    }

    public static Vector2 HeartCurve(float radius, float t)
    {
        t *= 2 * Mathf.PI;
        var x = 16 * Mathf.Pow(Mathf.Sin(t), 3);
        var y = 13 * Mathf.Cos(t) - 5 * Mathf.Cos(2 * t) - 2 * Mathf.Cos(3 * t) - Mathf.Cos(4 * t);
        return new Vector2(x, y) * radius;
    }
    public static float SmoothStep(float from,float to,float t)
    {
        t = Mathf.Clamp01(t);
        float v1 = t * t;
        float v2 = 1 - (1-t) *(1-t);
        return Mathf.Lerp(from,to, Mathf.Lerp(v1,v2,t));
    }
    public static Vector3 SmoothStep(Vector3 from,Vector3 to,float t)
    {
        t = Mathf.Clamp01(t);
        float v1 = t * t;
        float v2 = 1 - (1-t) *(1-t);
        return Vector3.Lerp(from,to, Mathf.Lerp(v1,v2,t));
    }

    /// <summary>
    /// 平滑跟踪
    /// </summary>
    /// <param name="current"></param>
    /// <param name="target"></param>
    /// <param name="k"></param>
    /// <param name="frameTime"></param>
    /// <returns></returns>
    public static Vector3 SmoothFollow(Vector3 current,Vector3 target,float k,float frameTime)
    {
        k = 1 - Mathf.Pow(k,frameTime);
        return Vector3.Lerp(current,target,k);
    }

    /// <summary>
    /// 绕着原点旋转一个向量angle角度
    /// </summary>
    /// <param name="vec"></param>
    /// <param name="angle">旋转的角度，弧度制</param>
    /// <returns></returns>
    public static Vector2 RotateVector2(Vector2 vec,float angle)
    {
        return new Vector2(vec.x * Mathf.Cos(angle) - vec.y * Mathf.Sin(angle), vec.x *  Mathf.Sin(angle) + vec.y * Mathf.Cos(angle));
    }

    /// <summary>
    /// 二次贝塞尔曲线Vector2版
    /// </summary>
    /// <param name="p0"></param>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    public static Vector2 QuadraticCurve(Vector2 p0, Vector2 p1, Vector2 p2, float t)
    {
        Vector2 a = Vector2.Lerp(p0, p1, t);
        Vector2 b = Vector2.Lerp(p1, p2, t);
        return Vector2.Lerp(a, b, t);
    }

    /// <summary>
    /// 三次贝塞尔曲线Vector2版
    /// </summary>
    /// <param name="p0"></param>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="p3"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    public static Vector2 CubicCurve(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
    {
        Vector2 a = QuadraticCurve(p0, p1, p2, t);
        Vector2 b = QuadraticCurve(p1, p2, p3, t);
        return Vector2.Lerp(a, b, t);
    }

    /// <summary>
    /// 二次贝塞尔曲线Vector3版
    /// </summary>
    /// <param name="p0"></param>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    public static Vector3 QuadraticCurve(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        Vector3 a = Vector3.Lerp(p0, p1, t);
        Vector3 b = Vector3.Lerp(p1, p2, t);
        return Vector3.Lerp(a, b, t);
    }

    /// <summary>
    /// 三次贝塞尔曲线Vector3版
    /// </summary>
    /// <param name="p0"></param>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="p3"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    public static Vector3 CubicCurve(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        Vector3 a = QuadraticCurve(p0, p1, p2, t);
        Vector3 b = QuadraticCurve(p1, p2, p3, t);
        return Vector3.Lerp(a, b, t);
    }
}
}