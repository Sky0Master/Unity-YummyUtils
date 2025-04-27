using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FolderEntry
{
    public string name;
    public string path;
}

public class PathBookmarkData : ScriptableObject
{
    public List<FolderEntry> folderList = new List<FolderEntry>();
}
