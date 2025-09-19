using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// 把SpriteSheet切分成单张图片文件保存在同目录下
/// </summary>
public class ImageSlicer {
    [MenuItem("Tools/Split Sprites")]
    static void ProcessToSprites() {
        Texture2D sourceImg = Selection.activeObject as Texture2D;
        string assetPath = AssetDatabase.GetAssetPath(sourceImg);
        
        TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        importer.isReadable = true; 
        AssetDatabase.ImportAsset(assetPath);
        
        string parentDir = Path.GetDirectoryName(assetPath);
        string outputDir = parentDir + "/" + sourceImg.name + "_Split";
        Directory.CreateDirectory(outputDir);

        foreach(SpriteMetaData meta in importer.spritesheet) {
            Rect rect = meta.rect;
            Texture2D subTexture = new Texture2D((int)rect.width, (int)rect.height);
            
            // 提取像素区域
            for(int y=(int)rect.y; y<rect.y+rect.height; y++) {
                for(int x=(int)rect.x; x<rect.x+rect.width; x++) {
                    subTexture.SetPixel(x-(int)rect.x, y-(int)rect.y, 
                        sourceImg.GetPixel(x,y));
                }
            }
            
            byte[] pngData = subTexture.EncodeToPNG();
            File.WriteAllBytes($"{outputDir}/{meta.name}.png", pngData);
        }

        AssetDatabase.Refresh(); // 刷新资源库[1,4](@ref)
    }
}