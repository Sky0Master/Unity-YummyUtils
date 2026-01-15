using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ObjectTransformSwitcher : MonoBehaviour
{
    [System.Serializable]
    public class TransformState
    {
        public string id;
        public Vector3 localPosition;
        public Vector3 localEulerAngles;
        public Vector3 localScale;

        // 构造函数，用于快速创建当前状态
        public TransformState(string id, Transform t)
        {
            this.id = id;
            this.localPosition = t.localPosition;
            this.localEulerAngles = t.localEulerAngles;
            this.localScale = t.localScale;
        }
    }

    [Header("状态列表")]
    public List<TransformState> states = new List<TransformState>();

    /// <summary>
    /// 平滑过渡到指定ID的状态 (使用 AnimationCurve)
    /// </summary>
    /// <param name="id">目标状态的ID</param>
    /// <param name="duration">过渡时间</param>
    /// <param name="animCurve">动画曲线</param>
    /// <param name="onComplete">完成后的回调</param>
    public void TransitionTo(string id, float duration, AnimationCurve animCurve, Action onComplete = null)
    {
        Sequence seq = CreateBaseSequence(id, duration);
        if (seq == null) return;

        // 应用动画曲线
        if (animCurve != null)
        {
            seq.SetEase(animCurve);
        }
        else
        {
            seq.SetEase(Ease.Linear); // 如果曲线为空，使用线性插值
        }

        // 绑定完成回调
        seq.OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }

    /// <summary>
    /// 平滑过渡到指定ID的状态 (使用 DOTween Ease)
    /// </summary>
    /// <param name="id">目标状态的ID</param>
    /// <param name="duration">过渡时间</param>
    /// <param name="ease">缓动类型，默认为 Linear</param>
    /// <param name="onComplete">完成后的回调</param>
    public void TransitionTo(string id, float duration, Ease ease = Ease.Linear, Action onComplete = null)
    {
        Sequence seq = CreateBaseSequence(id, duration);
        if (seq == null) return;

        // 应用 Ease
        seq.SetEase(ease);

        // 绑定完成回调
        seq.OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }

    // 提取公共的 Sequence 创建逻辑
    private Sequence CreateBaseSequence(string id, float duration)
    {
        // 1. 查找对应的状态
        TransformState targetState = states.Find(s => s.id == id);

        if (targetState == null)
        {
            Debug.LogWarning($"[ObjectTransformSwitcher] 找不到 ID 为 '{id}' 的状态！");
            return null;
        }

        // 2. 停止当前物体上正在进行的所有 DOTween 动画，防止冲突
        transform.DOKill();

        // 3. 创建 DOTween 序列 (Sequence) 来同时管理位移、旋转和缩放
        Sequence seq = DOTween.Sequence();
        
        // 【关键】将序列的目标设为 transform，这样 transform.DOComplete() 才能控制这个序列的回调
        seq.SetTarget(transform);

        // 添加位移动画 (局部坐标)
        seq.Join(transform.DOLocalMove(targetState.localPosition, duration));

        // 添加旋转动画 (局部欧拉角)
        seq.Join(transform.DOLocalRotate(targetState.localEulerAngles, duration));

        // 添加缩放动画
        seq.Join(transform.DOScale(targetState.localScale, duration));

        return seq;
    }

    /// <summary>
    /// 立即停止当前动画，并将物体状态重置回动画的【起点】
    /// </summary>
    public void StopAndResetToStart()
    {
        // Rewind 将动画倒带回开始状态
        transform.DORewind();
        // Kill 移除动画实例
        transform.DOKill();
    }

    /// <summary>
    /// 立即停止当前动画，并将物体状态跳转到动画的【终点】
    /// </summary>
    public void StopAndSnapToEnd()
    {
        // Complete 将动画直接跳转到结束状态，并且会触发 OnComplete 回调
        transform.DOComplete();
        // Kill 移除动画实例
        transform.DOKill();
    }
}

// =========================================================
// 编辑器扩展部分 (仅在 Unity 编辑器中运行)
// =========================================================
#if UNITY_EDITOR

// 1. 自定义 PropertyDrawer，用于美化 List 中的每个元素并添加 Apply 按钮
[CustomPropertyDrawer(typeof(ObjectTransformSwitcher.TransformState))]
public class TransformStateDrawer : PropertyDrawer
{
    // 定义行高和间距
    private float lineHeight = EditorGUIUtility.singleLineHeight;
    private float spacing = 2f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // 获取 ID 属性，用于显示在标题栏
        SerializedProperty idProp = property.FindPropertyRelative("id");
        string title = string.IsNullOrEmpty(idProp.stringValue) ? label.text : idProp.stringValue;

        // 计算区域
        // 我们需要放置两个按钮，假设每个按钮宽 45-50 像素
        float btnWidth = 48f;
        float btnSpacing = 2f;
        float rightMargin = (btnWidth * 2) + btnSpacing;

        // 标题栏区域 (留出右侧给两个按钮)
        Rect headerRect = new Rect(position.x, position.y, position.width - rightMargin - 5, lineHeight);
        
        // 按钮区域
        Rect applyBtnRect = new Rect(position.x + position.width - rightMargin, position.y, btnWidth, lineHeight);
        Rect saveBtnRect = new Rect(position.x + position.width - btnWidth, position.y, btnWidth, lineHeight);

        // 绘制折叠箭头和标题
        property.isExpanded = EditorGUI.Foldout(headerRect, property.isExpanded, title, true);

        // 绘制 "Apply" 按钮 (预览：State -> Transform)
        if (GUI.Button(applyBtnRect, "Apply"))
        {
            ApplyState(property);
        }

        // 绘制 "Save" 按钮 (更新：Transform -> State)
        // 使用不同的颜色提示这是一个写入操作
        Color oldColor = GUI.backgroundColor;
        GUI.backgroundColor = new Color(0.9f, 1f, 0.9f); // 淡绿色
        if (GUI.Button(saveBtnRect, "Save"))
        {
            UpdateState(property);
        }
        GUI.backgroundColor = oldColor;

        // 如果展开，绘制属性内容
        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;
            
            Rect contentRect = new Rect(position.x, position.y + lineHeight + spacing, position.width, lineHeight);
            
            // 绘制 ID
            EditorGUI.PropertyField(contentRect, property.FindPropertyRelative("id"));
            contentRect.y += lineHeight + spacing;
            
            // 绘制 Position
            EditorGUI.PropertyField(contentRect, property.FindPropertyRelative("localPosition"));
            contentRect.y += lineHeight + spacing;
            
            // 绘制 Rotation
            EditorGUI.PropertyField(contentRect, property.FindPropertyRelative("localEulerAngles"));
            contentRect.y += lineHeight + spacing;
            
            // 绘制 Scale
            EditorGUI.PropertyField(contentRect, property.FindPropertyRelative("localScale"));
            
            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // 如果折叠，只返回一行高度
        if (!property.isExpanded)
        {
            return lineHeight;
        }
        
        // 如果展开，返回 标题 + 4个字段 + 间距 的总高度
        return (lineHeight + spacing) * 5; 
    }

    // 将存储的状态应用到物体上 (Read State -> Write Transform)
    private void ApplyState(SerializedProperty property)
    {
        // 获取目标脚本对象
        UnityEngine.Object targetObject = property.serializedObject.targetObject;
        ObjectTransformSwitcher script = targetObject as ObjectTransformSwitcher;

        if (script != null)
        {
            // 获取属性值
            Vector3 pos = property.FindPropertyRelative("localPosition").vector3Value;
            Vector3 rot = property.FindPropertyRelative("localEulerAngles").vector3Value;
            Vector3 scale = property.FindPropertyRelative("localScale").vector3Value;

            // 记录 Undo 操作，这样按 Ctrl+Z 可以撤销
            Undo.RecordObject(script.transform, "Apply Transform State");

            // 应用变换
            script.transform.localPosition = pos;
            script.transform.localEulerAngles = rot;
            script.transform.localScale = scale;
            
            Debug.Log($"[Editor] 已应用状态: {property.FindPropertyRelative("id").stringValue}");
        }
    }

    // 将当前物体的 Transform 值覆盖到此状态中 (Read Transform -> Write State)
    private void UpdateState(SerializedProperty property)
    {
        UnityEngine.Object targetObject = property.serializedObject.targetObject;
        ObjectTransformSwitcher script = targetObject as ObjectTransformSwitcher;

        if (script != null)
        {
            // 直接修改 SerializedProperty 的子属性
            // SerializedProperty 的修改会自动处理 Undo（只要之后调用 ApplyModifiedProperties）
            
            property.FindPropertyRelative("localPosition").vector3Value = script.transform.localPosition;
            property.FindPropertyRelative("localEulerAngles").vector3Value = script.transform.localEulerAngles;
            property.FindPropertyRelative("localScale").vector3Value = script.transform.localScale;

            // 关键：应用修改。这会确保数据被写回对象，并且标记为已修改(Dirty)，同时注册 Undo。
            property.serializedObject.ApplyModifiedProperties();
            
            Debug.Log($"[Editor] 已更新状态 '{property.FindPropertyRelative("id").stringValue}' 为当前物体变换值");
        }
    }
}

// 2. 自定义 Editor，用于在 Inspector 底部添加录制按钮
[CustomEditor(typeof(ObjectTransformSwitcher))]
public class ObjectTransformSwitcherEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ObjectTransformSwitcher script = (ObjectTransformSwitcher)target;

        // 绘制默认的 Inspector
        // 注意：因为我们上面定义了 TransformStateDrawer，这里绘制 List 时会自动使用新的样式
        DrawDefaultInspector();

        GUILayout.Space(10);

        // 绘制录制按钮
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Record Current Transform as New State", GUILayout.Height(30)))
        {
            RecordNewState(script);
        }
        GUI.backgroundColor = Color.white;
    }

    void RecordNewState(ObjectTransformSwitcher script)
    {
        string newName = "State_" + (script.states.Count > 0 ? script.states.Count.ToString() : "A");
        
        ObjectTransformSwitcher.TransformState newState = new ObjectTransformSwitcher.TransformState(newName, script.transform);

        // 必须使用 Undo 记录列表的更改，否则无法撤销且可能不会标记为 Dirty
        Undo.RecordObject(script, "Record New State");
        
        script.states.Add(newState);

        // 强制刷新 Inspector
        EditorUtility.SetDirty(script);
        Debug.Log($"已记录当前状态: {newName}");
    }
}
#endif