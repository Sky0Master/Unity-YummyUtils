using UnityEngine;
using TMPro;

namespace Yummy{
[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform), typeof(TextMeshProUGUI))]
public class TMPAutoSize : MonoBehaviour
{
    [Header("配置参数")]
    [Tooltip("最小高度")] public float minHeight = 20f;
    [Tooltip("最大高度")] public float maxHeight = 500f;
    [Tooltip("边缘留白")] public float padding = 5f;

    private TextMeshProUGUI _tmp;
    private RectTransform _rectTransform;
    private string _lastText;
    private float _lastWidth;

    void Awake()
    {
        InitializeComponents();
        UpdateSize(true);
    }
    void Update()
    {
        #if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            InitializeComponents();
        }
        #endif

        CheckForChanges();
    }
    #if UNITY_EDITOR
    void OnValidate()
    {
        UpdateSize(true);
    }
    #endif
    private void InitializeComponents()
    {
        if (_tmp == null) _tmp = GetComponent<TextMeshProUGUI>();
        if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();
    }

    private void CheckForChanges()
    {
        bool widthChanged = !Mathf.Approximately(_rectTransform.rect.width, _lastWidth);
        bool textChanged = _tmp.text != _lastText;

        if (widthChanged || textChanged)
        {
            UpdateSize();
            _lastText = _tmp.text;
            _lastWidth = _rectTransform.rect.width;
        }
    }
    public void UpdateSize(bool forceUpdate = false)
    {
        if (!isActiveAndEnabled) return;

        _tmp.ForceMeshUpdate();
        
        float preferredHeight = _tmp.preferredHeight + padding * 2;
        float newHeight = Mathf.Clamp(preferredHeight, minHeight, maxHeight);

        if (!Mathf.Approximately(_rectTransform.sizeDelta.y, newHeight) || forceUpdate)
        {
            Vector2 newSize = new Vector2(_rectTransform.sizeDelta.x, newHeight);
            _rectTransform.sizeDelta = newSize;
            
            // 保持文本顶部对齐
            if (_tmp.verticalAlignment == VerticalAlignmentOptions.Top)
            {
                Vector3 pos = _rectTransform.localPosition;
                pos.y = -newHeight / 2;
                _rectTransform.localPosition = pos;
            }
        }
    }
}

}