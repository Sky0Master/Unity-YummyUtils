using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

/// <summary>
/// 需要Dotween
/// </summary>
public class UIImageSwitch : MonoBehaviour, IPointerClickHandler
{
    public Sprite[] sprites;
    int _currentIndex = 0;
    private Image img;
    private bool isAnimating = false; // 动画状态标志
    public UnityEvent OnEnd;
    [Header("Fade Effect")]
    public bool enableFadeEffect = true; // 是否启用淡入淡出效果
    public float minAlpha = 0.5f; // 最小透明度
    public float minScale = 0.9f; // 最小缩放
    public float duration = 0.2f; // 动画持续时间
    public void OnPointerClick(PointerEventData eventData)
    {
        if (isAnimating) return;

        if(_currentIndex < sprites.Length - 1)
        {
            _currentIndex++;
            if(enableFadeEffect)
                FadeSwitch();
            else
                SetImage();   
        }
        else{
            OnEnd?.Invoke();
            img.enabled = false;
        }
    }
    public void FadeSwitch()
    {
        isAnimating = true;
        
        // 当前图片的退出动画：缩小 + 淡出
        img.transform.DOScale(new Vector3(minScale, minScale, 1), duration);
        img.DOFade(minAlpha, duration).OnComplete(() =>
        {
            SetImage();   
            // 新图片的进入动画：放大 + 淡入
            img.color = new Color(1, 1, 1, minAlpha);
            img.transform.DOScale(Vector3.one, duration);
            img.DOFade(1, duration).OnComplete(() => isAnimating = false);
        });
        
    }
    public void SetImage()
    {
        if(img == null)
            return;
        
        img.sprite = sprites[_currentIndex];
        //img.SetNativeSize();
    }

    public void ResetImage()
    {
        img.enabled = true;
        _currentIndex = 0;
        SetImage();
        
        // 重置动画相关属性
        img.transform.localScale = Vector3.one;
        img.color = Color.white;
    }

    private void Start() {
        img = GetComponent<Image>();
        ResetImage();
    }
}
