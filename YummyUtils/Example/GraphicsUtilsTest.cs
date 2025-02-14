using UnityEngine;

public class GraphicsUtilsTest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Yummy.GraphicsUtils.TakeScreenshot("test", new Vector2Int(1920, 1080), Camera.main);
    }

    private void OnGUI() {
        var rect = new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y, 100, 100);
        rect.x -= 50;
        rect.y -= 50;
        Yummy.GraphicsUtils.DrawScreenRect(rect, Color.red);    
    }
}
