using System;
using Core.UI.Windows.Contracts;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.UI.Windows.Components
{
    public abstract class BaseWindow : MonoBehaviour, IWindow
    {
        [SerializeField] private RectTransform rootRectTransform;
        [SerializeField] private CanvasGroup canvasGroup;

        private const float FadeInDuration = 0.35f;
        private const float FadeOutDuration = 0.45f;
        private const float MaxAnimationDeltaTime = 1f / 20f;
        
        public RectTransform RootRectTransform => rootRectTransform;
        public bool IsActive { get; private set; }
        public bool IsInteractable => canvasGroup == null || canvasGroup.interactable;

        public event Action<IWindow> Destroyed;

        public virtual async UniTask ShowAsync()
        {
            gameObject.SetActive(true);
            
            await AnimateAlphaAsync(0f, 1f, FadeInDuration);
            
            if (canvasGroup != null)
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }

            IsActive = true;
        }

        public virtual async UniTask HideAsync()
        {
            IsActive = false;

            if (canvasGroup != null)
            {
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
            
            float startAlpha = canvasGroup.alpha;
            if (startAlpha <= 0.001f)
            {
                gameObject.SetActive(false);
                return;
            }
            await AnimateAlphaAsync(startAlpha, 0f, FadeOutDuration);
            
            gameObject.SetActive(false);
        }

        public virtual void ShowInstantly()
        {
            gameObject.SetActive(true);

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }
            
            IsActive = true;
        }

        public virtual void HideInstantly()
        {
            IsActive = false;

            if (canvasGroup != null)
            {
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
                canvasGroup.alpha = 0f;
            }

            gameObject.SetActive(false);
        }

        protected virtual void OnDestroy()
        {
            Destroyed?.Invoke(this);
        }
        
        private async UniTask AnimateAlphaAsync(float from, float to, float duration)
        {
            canvasGroup.alpha = from;
            await UniTask.Yield(PlayerLoopTiming.PostLateUpdate);

            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Mathf.Min(Time.unscaledDeltaTime, MaxAnimationDeltaTime);
                float t = Mathf.Clamp01(elapsed / duration);
                float smoothed = SmoothStep(t);
                canvasGroup.alpha = Mathf.Lerp(from, to, smoothed);
                await UniTask.Yield(PlayerLoopTiming.PostLateUpdate);
            }

            canvasGroup.alpha = to;
        }

        private static float SmoothStep(float t) => t * t * (3f - 2f * t);
    }
}