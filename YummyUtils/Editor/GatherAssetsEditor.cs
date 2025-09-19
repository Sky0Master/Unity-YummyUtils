using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 一键把选中的Prefab里所引用的所有资源文件全部移动到当前文件夹
/// </summary>
public class GatherAssetsEditor : EditorWindow
{
    private DefaultAsset selectedFolder; // To hold the target folder if user specifies

    [MenuItem("Assets/Move Referenced Assets Here")]
    public static void ShowWindow()
    {
        GetWindow<GatherAssetsEditor>("Move Referenced Assets");
    }

    private void OnGUI()
    {
        GUILayout.Label("Move Referenced Assets to Selected Folder", EditorStyles.boldLabel);

        EditorGUILayout.HelpBox("Select a Prefab or other asset in the Project window. " +
                                "This script will find all assets (excluding C# scripts) " +
                                "referenced by the selected asset and move them to the " +
                                "same folder as the selected asset.", MessageType.Info);

        if (GUILayout.Button("Execute Move"))
        {
            MoveAssets();
        }
    }

    private static void MoveAssets()
    {
        Object selectedAsset = Selection.activeObject;

        if (selectedAsset == null)
        {
            EditorUtility.DisplayDialog("Error", "No asset selected. Please select an asset in the Project window.", "OK");
            return;
        }

        string selectedAssetPath = AssetDatabase.GetAssetPath(selectedAsset);
        if (string.IsNullOrEmpty(selectedAssetPath))
        {
            EditorUtility.DisplayDialog("Error", "Could not get path for the selected asset.", "OK");
            return;
        }

        string targetFolderPath = Path.GetDirectoryName(selectedAssetPath);

        if (string.IsNullOrEmpty(targetFolderPath))
        {
            EditorUtility.DisplayDialog("Error", "Could not determine the target folder path.", "OK");
            return;
        }

        HashSet<Object> referencedAssets = new HashSet<Object>();

        // Get all direct and indirect references
        Object[] directReferences = EditorUtility.CollectDependencies(new Object[] { selectedAsset });

        foreach (Object obj in directReferences)
        {
            // Exclude the selected asset itself and C# scripts
            if (obj != selectedAsset && !(obj is MonoScript))
            {
                string assetPath = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(assetPath) && AssetDatabase.Contains(obj))
                {
                    referencedAssets.Add(obj);
                }
            }
        }

        if (referencedAssets.Count == 0)
        {
            EditorUtility.DisplayDialog("Info", "No other assets (excluding C# scripts) referenced by the selected asset found to move.", "OK");
            return;
        }

        int movedCount = 0;
        List<string> movedAssetNames = new List<string>();
        AssetDatabase.StartAssetEditing(); // Group asset moves for better performance

        try
        {
            foreach (Object assetToMove in referencedAssets)
            {
                string currentAssetPath = AssetDatabase.GetAssetPath(assetToMove);
                if (string.IsNullOrEmpty(currentAssetPath)) continue;

                string assetFileName = Path.GetFileName(currentAssetPath);
                string newAssetPath = Path.Combine(targetFolderPath, assetFileName);

                // Prevent moving an asset to its current location or if it's already in the target folder
                if (Path.GetDirectoryName(currentAssetPath) == targetFolderPath)
                {
                    Debug.Log($"Asset '{assetFileName}' is already in the target folder. Skipping move.");
                    continue;
                }

                string result = AssetDatabase.MoveAsset(currentAssetPath, newAssetPath);

                if (string.IsNullOrEmpty(result)) // result is empty string on success
                {
                    movedCount++;
                    movedAssetNames.Add(assetFileName);
                    Debug.Log($"Moved '{assetFileName}' from '{currentAssetPath}' to '{newAssetPath}'");
                }
                else
                {
                    Debug.LogError($"Failed to move '{assetFileName}': {result}");
                }
            }
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
            AssetDatabase.Refresh(); // Refresh the AssetDatabase to show changes
        }

        if (movedCount > 0)
        {
            EditorUtility.DisplayDialog("Success", $"Successfully moved {movedCount} asset(s) to '{targetFolderPath}'.\n\nMoved Assets:\n{string.Join("\n", movedAssetNames)}", "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("Info", "No new assets were moved.", "OK");
        }
    }
}