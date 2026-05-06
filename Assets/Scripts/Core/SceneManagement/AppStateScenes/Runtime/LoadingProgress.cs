using System;
using Core.SceneManagement.AppStateScenes.Contracts;
using Core.SceneManagement.AppStateScenes.Data;
using UnityEngine;

namespace Core.SceneManagement.AppStateScenes.Runtime
{
    public sealed class LoadingProgress : ILoadingProgress
    {
        public event Action<LoadingProgressInfo> Changed;

        public LoadingProgressInfo Current { get; private set; } =
            new(0f, "Loading...");

        public void Reset(string message = "Loading...")
        {
            Report(0f, message);
        }

        public void Report(float progress, string message = null)
        {
            Current = new LoadingProgressInfo(
                Mathf.Clamp01(progress),
                string.IsNullOrWhiteSpace(message) ? Current.Message : message);

            Changed?.Invoke(Current);
        }
    }
}