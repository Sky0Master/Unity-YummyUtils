using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

// 定义一个新的编辑器窗口
public class DraggableAssetReferenceWindow : EditorWindow
{
    // 用于存储被拖拽到窗口中的资源引用
    // Use a list to store the referenced objects
    private List<Object> referencedObjects = new List<Object>();
    
    // 滚动视图的位置
    // Scroll view position for the list of objects
    private Vector2 scrollPosition;

    [MenuItem("Window/Draggable Asset References")]
    public static void ShowWindow()
    {
        // 创建并显示窗口
        // Create and show the new window
        GetWindow<DraggableAssetReferenceWindow>("Asset References");
    }

    private void OnGUI()
    {
        // 窗口标题
        // Window title
        EditorGUILayout.LabelField("Draggable Asset References", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // 处理拖拽文件到窗口中的操作
        // Handle dropping files into the window
        HandleDragAndDropIn();

        // 绘制资源列表
        // Draw the list of referenced objects
        DrawObjectList();

        // 绘制一个清除列表的按钮
        // Draw a button to clear the list
        if (GUILayout.Button("Clear All References"))
        {
            referencedObjects.Clear();
        }
    }

    private void DrawObjectList()
    {
        // 使用滚动视图来显示列表
        // Use a scroll view to display the list of objects
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true));
        
        // 如果列表为空，显示提示信息
        // If the list is empty, display a helpful message
        if (referencedObjects.Count == 0)
        {
            EditorGUILayout.HelpBox("Drag and drop assets from the Project window here.", MessageType.Info);
        }

        // 遍历并绘制列表中的每个资源
        // Iterate and draw each object in the list
        for (int i = 0; i < referencedObjects.Count; i++)
        {
            var obj = referencedObjects[i];
            
            if (obj == null)
            {
                // 如果资源被删除，从列表中移除
                // Remove the object from the list if it has been deleted
                referencedObjects.RemoveAt(i);
                i--;
                continue;
            }

            // 获取当前事件
            // Get the current event
            var currentEvent = Event.current;
            
            // 为每个资源创建一个可点击的区域
            // Create a clickable area for each object
            Rect rect = EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(obj.name);
            EditorGUILayout.EndHorizontal();

            // 如果鼠标在当前资源的区域内，并且事件类型是鼠标按下
            // If the mouse is within the current object's area and the event is a mouse down
            if (rect.Contains(currentEvent.mousePosition) && currentEvent.type == EventType.MouseDown)
            {
                // 如果是双击
                // If it's a double-click
                if (currentEvent.clickCount == 2)
                {
                    // 打开选中的资源，例如：打开预制体编辑器或文件
                    // Open the selected asset (e.g., open prefab editor or file)
                    AssetDatabase.OpenAsset(obj);
                    currentEvent.Use(); // 消费事件
                }
                // 如果是开始拖拽
                // If it's the start of a drag operation
                else if (currentEvent.clickCount == 1 && currentEvent.button == 0)
                {
                    // 准备开始拖拽操作，以便可以将资源拖出窗口
                    // Prepare to start a drag operation to allow dragging the asset out of the window
                    DragAndDrop.PrepareStartDrag();
                    DragAndDrop.objectReferences = new Object[] { obj };
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    DragAndDrop.StartDrag(obj.name);
                    currentEvent.Use(); // 消费事件
                }
            }
        }
        EditorGUILayout.EndScrollView();
    }

    private void HandleDragAndDropIn()
    {
        // 检查当前事件类型
        // Check the current event type
        var currentEvent = Event.current;
        var dropArea = GUILayoutUtility.GetRect(0, 50, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, "Drop Assets Here", EditorStyles.helpBox);

        switch (currentEvent.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                // 如果鼠标在拖拽区域内
                // If the mouse is within the drop area
                if (!dropArea.Contains(currentEvent.mousePosition))
                {
                    return;
                }

                // 设置拖拽操作的可视模式，显示为可以复制
                // Set the drag and drop visual mode to copy
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (currentEvent.type == EventType.DragPerform)
                {
                    // 接受拖拽操作
                    // Accept the drag and drop operation
                    DragAndDrop.AcceptDrag();

                    // 将拖拽的资源添加到列表中
                    // Add the dropped objects to the list
                    foreach (var draggedObject in DragAndDrop.objectReferences)
                    {
                        // 确保只添加项目中的资源，而不是场景中的物体
                        // Ensure we only add assets from the project, not scene objects
                        if (AssetDatabase.Contains(draggedObject))
                        {
                            if (!referencedObjects.Contains(draggedObject))
                            {
                                referencedObjects.Add(draggedObject);
                            }
                        }
                    }
                    currentEvent.Use(); // 消费事件
                }
                break;
        }
    }
}
