using System;
using UnityEngine;

namespace Core.SceneManagement.AppStateScenes.Data
{
    public readonly struct LoadingProgressInfo
    {
        public float Progress { get; }
        public string Message { get; }

        public LoadingProgressInfo(float progress, string message)
        {
            Progress = Mathf.Clamp01(progress);
            Message = message;
        }
    }
}