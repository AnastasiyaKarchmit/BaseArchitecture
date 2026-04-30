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

        public RectTransform RootRectTransform => rootRectTransform;
        public bool IsActive { get; private set; }
        public bool IsInteractable => canvasGroup == null || canvasGroup.interactable;

        public event Action<IWindow> Destroyed;

        public virtual UniTask ShowAsync()
        {
            gameObject.SetActive(true);
            IsActive = true;

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }

            return UniTask.CompletedTask;
        }

        public virtual UniTask HideAsync()
        {
            IsActive = false;

            if (canvasGroup != null)
            {
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
                canvasGroup.alpha = 0f;
            }

            gameObject.SetActive(false);
            return UniTask.CompletedTask;
        }

        protected virtual void OnDestroy()
        {
            Destroyed?.Invoke(this);
        }
    }
}