using UnityEditor;
using UnityEngine;
using System.IO;

public class BookmarkWindowEditor : EditorWindow
{
    private PathBookmarkData data;
    private Vector2 scrollPos;
    private int renameIndex = -1;
    private string renameText = "";

    private const string DATA_PATH = "Assets/Editor/BookmarkData.asset";

    [MenuItem("Tools/Path Bookmark")]
    public static void ShowWindow()
    {
        GetWindow<BookmarkWindowEditor>("Path Bookmark");
    }

    private void OnEnable()
    {
        LoadOrCreateData();
    }

    private void LoadOrCreateData()
    {
        data = AssetDatabase.LoadAssetAtPath<PathBookmarkData>(DATA_PATH);

        if (data == null)
        {
            data = ScriptableObject.CreateInstance<PathBookmarkData>();

            if (!Directory.Exists("Assets/Editor"))
                Directory.CreateDirectory("Assets/Editor");

            AssetDatabase.CreateAsset(data, DATA_PATH);
            AssetDatabase.SaveAssets();
        }
    }

    private void SaveData()
    {
        EditorUtility.SetDirty(data);
        AssetDatabase.SaveAssets();
    }

    private void OnGUI()
    {
        GUILayout.Label("书签列表", EditorStyles.boldLabel);

        if (data == null) return;

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        for (int i = 0; i < data.folderList.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            if (renameIndex == i)
            {
                renameText = EditorGUILayout.TextField(renameText);

                if (GUILayout.Button("保存", GUILayout.Width(50)))
                {
                    data.folderList[i].name = renameText;
                    renameIndex = -1;
                    SaveData();
                }

                if (GUILayout.Button("取消", GUILayout.Width(50)))
                {
                    renameIndex = -1;
                }
            }
            else
            {
                if (GUILayout.Button(data.folderList[i].name, GUILayout.ExpandWidth(true)))
                {
                    var folderObj = AssetDatabase.LoadAssetAtPath<Object>(data.folderList[i].path);
                    if (folderObj != null)
                    {
                        Selection.activeObject = folderObj;
                        EditorGUIUtility.PingObject(folderObj);
                    }
                }

                if (GUILayout.Button("重命名", GUILayout.Width(60)))
                {
                    renameIndex = i;
                    renameText = data.folderList[i].name;
                }

                if (GUILayout.Button("X", GUILayout.Width(25)))
                {
                    data.folderList.RemoveAt(i);
                    SaveData();
                    break;
                }
            }

            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();

        GUILayout.Space(10);

        if (GUILayout.Button("添加选中项"))
        {
            bool added = false;
            foreach (var obj in Selection.objects)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                if (!data.folderList.Exists(f => f.path == path))
                {
                    data.folderList.Add(new FolderEntry { name = Path.GetFileName(path), path = path });
                    added = true;
                }
            }
            if (added)
            {
                SaveData();
            }
        }
    }
}
