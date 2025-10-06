using DG.Tweening;
using UnityEngine;

public interface IShowAndHide
{
    void Show();
    void Hide();
    void ShowImmediately();
    void HideImmediately();
}

public class VinoShowAndHide : MonoBehaviour, IShowAndHide
{
    [Space(5)]
    [Header("Show and Hide Settings")]
    public float showDuration = 0.2f;
    public float hideDuration = 0.2f;
    public Ease easeType = Ease.OutBack;

    private bool _isShow = true;

    public void Hide()
    {
        if (!_isShow) return;
        transform.DOScale(Vector3.zero, hideDuration).SetEase(easeType);
        _isShow = false;
    }

    public void HideImmediately()
    {
        if (!_isShow) return;
        transform.localScale = Vector3.zero;
        _isShow = false;
    }

    public void Show()
    {
        if (_isShow) return;
        transform.DOScale(Vector3.one, showDuration).SetEase(easeType);
        _isShow = true;
    }

    public void ShowImmediately()
    {
        if (_isShow) return;
        transform.localScale = Vector3.one;
        _isShow = true;
    }
}