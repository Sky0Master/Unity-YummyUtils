using UnityEngine;
using TMPro;
using DG.Tweening;

namespace Yummy
{
    /// <summary>
    /// VinoLabel is a custom label component that supports various text change animations and Font settings.
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class VinoLabel : MonoBehaviour
    {
        private TextMeshProUGUI _tmp;
        private Vector3 _originalScale;
        private Vector3 _originalPosition;
        private Quaternion _originalRotation;
        private CanvasGroup _canvasGroup;

        public ChangeAnimationType changeAnimationType = ChangeAnimationType.None;

        public enum ChangeAnimationType
        {
            None,       // 无动画
            Scale,      // 缩放动画
            Pull,       // 下拉回弹
            Fade,       // 淡出淡入
            Punch,      // 抖动弹出感
            RotateY     // Y轴旋转切换（类似翻页）
        }

        private void Awake()
        {
            _tmp = GetComponent<TextMeshProUGUI>();
            _originalScale = _tmp.transform.localScale;
            _originalPosition = _tmp.transform.localPosition;
            _originalRotation = _tmp.transform.localRotation;
            //添加 CanvasGroup 以支持 Fade
            _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup == null)
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        public void SetColor(Color color)
        {
            _tmp.color = color;
        }

        public string GetText()
        {
            return _tmp.text;
        }

        public void SetText(string newText)
        {
            switch (changeAnimationType)
            {
                case ChangeAnimationType.None:
                    _tmp.text = newText;
                    break;
                case ChangeAnimationType.Scale:
                    AnimateScale(newText);
                    break;
                case ChangeAnimationType.Pull:
                    AnimatePull(newText);
                    break;
                case ChangeAnimationType.Fade:
                    AnimateFade(newText);
                    break;
                case ChangeAnimationType.Punch:
                    AnimatePunch(newText);
                    break;
                case ChangeAnimationType.RotateY:
                    AnimateRotateY(newText);
                    break;
            }
        }

        private void AnimateScale(string newText)
        {
            _tmp.transform.DOScale(_originalScale * 0.4f, 0.1f).SetEase(Ease.InBack).OnComplete(() =>
            {
                _tmp.text = newText;
                _tmp.transform.DOScale(_originalScale, 0.25f).SetEase(Ease.OutElastic);
            });
        }

        private void AnimatePull(string newText)
        {
            float pullDistance = 20f;
            _tmp.transform.DOLocalMoveY(_originalPosition.y - pullDistance, 0.15f).SetEase(Ease.InBack).OnComplete(() =>
            {
                _tmp.text = newText;
                _tmp.transform.DOLocalMoveY(_originalPosition.y, 0.3f).SetEase(Ease.OutElastic);
            });
        }

        private void AnimateFade(string newText)
        {
            _canvasGroup.DOFade(0f, 0.2f).OnComplete(() =>
            {
                _tmp.text = newText;
                _canvasGroup.DOFade(1f, 0.3f);
            });
        }

        private void AnimatePunch(string newText)
        {
            _tmp.text = newText;
            _tmp.transform.DOPunchScale(Vector3.one * 0.25f, 0.4f, 8, 1);
        }

        private void AnimateRotateY(string newText)
        {
            _tmp.transform.DORotate(new Vector3(0, 90, 0), 0.15f).SetEase(Ease.InQuad).OnComplete(() =>
            {
                _tmp.text = newText;
                _tmp.transform.DORotate(new Vector3(0, 0, 0), 0.3f).SetEase(Ease.OutBack);
            });
        }

        private void OnDisable()
        {
            _tmp.transform.localScale = _originalScale;
            _tmp.transform.localPosition = _originalPosition;
            _tmp.transform.localRotation = _originalRotation;
            if (_canvasGroup != null) _canvasGroup.alpha = 1f;
        }
    }
}