using UnityEngine;
using System.IO;

namespace Yummy {
public static class GraphicsUtils
{
    /// <summary>
    /// 截取指定相机的画面
    /// </summary>
    /// <param name="name">文件名</param>
    /// <param name="size">截图分辨率</param>
    /// <param name="camera">截图的相机</param>
    /// <param name="folderPath">保存路径，默认在/Resources/Captures</param>
    public static void TakeScreenshot(string name, Vector2Int size, Camera camera, string folderPath = "")
    {
        RenderTexture prevRt = camera.targetTexture;

        // prepare render
        RenderTexture rt = new RenderTexture(size.x, size.y, 24, RenderTextureFormat.ARGB32);
        rt.antiAliasing = 4;

        camera.targetTexture = rt;
        camera.Render();

        // transfer to Texture2D and bytes
        Texture2D output = new Texture2D(size.x, size.y, TextureFormat.RGB24, false);

        RenderTexture.active = rt;
        output.ReadPixels(new Rect(0, 0, size.x, size.y), 0, 0, false);

        byte[] bytes = output.EncodeToJPG(90);
        Object.DestroyImmediate(output);

        // write to a file in the project folder
        if (folderPath == "")
        {
            folderPath = Application.dataPath + "/Resources/Captures";
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
        }
        string filePath = $"{folderPath}/{name}.jpg";
        File.WriteAllBytes(filePath, bytes);

        // clean
        RenderTexture.active = null;
        camera.targetTexture = prevRt;
        rt.DiscardContents();

        Debug.Log($"Took screenshot: {filePath}");
    }

     static Texture2D _whiteTexture;
    public static Texture2D WhiteTexture
    {
        get
        {
            if (_whiteTexture == null)
            {
                _whiteTexture = new Texture2D(1, 1);
                _whiteTexture.SetPixel(0, 0, Color.white);
                _whiteTexture.Apply();
            }

            return _whiteTexture;
        }
    }
    public static void DrawScreenRect(Rect rect, Color color)
    {
        GUI.color = color;
        GUI.DrawTexture(rect, WhiteTexture);
        GUI.color = Color.white;
    }

    public static void DrawScreenRectBorder(Rect rect, float thickness, Color color)
    {
        // Top
        DrawScreenRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
        // Left
        DrawScreenRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
        // Right
        DrawScreenRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
        // Bottom
        DrawScreenRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
    }
}
}