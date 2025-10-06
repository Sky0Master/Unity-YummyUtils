using UnityEngine;
using UnityEngine.EventSystems; // Required for PointerDownHandler and PointerUpHandler
using DG.Tweening;             // Required for DOTween animations

public class UIClickEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Effect Settings")]
    [Tooltip("点击时UI收缩到的目标比例 (例如: 0.9表示收缩到原始大小的90%)")]
    [Range(0.1f, 1.0f)] // 限制收缩比例在0.1到1之间
    public float shrinkScale = 0.95f; 

    [Tooltip("收缩动画的持续时间")]
    public float shrinkDuration = 0.1f;

    [Tooltip("恢复原始大小动画的持续时间")]
    public float restoreDuration = 0.1f;

    [Tooltip("动画的缓动类型")]
    public Ease easeType = Ease.OutQuad; // OutQuad 通常提供平滑的加速和减速效果

    private Vector3 originalScale; // 存储UI的原始大小
    private Tween currentTween;    // 用于管理当前的DOTween动画，防止动画冲突

    void Awake()
    {
        // 在Awake中获取原始大小，确保在任何动画开始前记录
        originalScale = transform.localScale;
    }

    // 当鼠标按下或手指触摸到UI时调用
    public void OnPointerDown(PointerEventData eventData)
    {
        // 停止任何正在进行的动画，避免冲突
        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Kill(true); // Kill(true) 强制完成当前动画到终点
        }

        // 使用DOTween进行收缩动画
        currentTween = transform.DOScale(originalScale * shrinkScale, shrinkDuration)
                                .SetEase(easeType)
                                .OnComplete(() => {
                                    // 动画完成后不立即恢复，等待PointerUp
                                });
    }

    // 当鼠标抬起或手指离开UI时调用
    public void OnPointerUp(PointerEventData eventData)
    {
        // 停止任何正在进行的动画
        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Kill(true);
        }

        // 使用DOTween恢复原始大小动画
        currentTween = transform.DOScale(originalScale, restoreDuration)
                                .SetEase(easeType);
    }

    // 当GameObject被禁用或销毁时，确保Tween被清理，避免内存泄漏
    void OnDisable()
    {
        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Kill(); // Kill() 直接停止并清理动画
        }
    }

    void OnDestroy()
    {
        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Kill();
        }
    }
}