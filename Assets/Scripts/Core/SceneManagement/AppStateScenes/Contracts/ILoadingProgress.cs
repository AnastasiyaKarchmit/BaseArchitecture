using System;
using Core.SceneManagement.AppStateScenes.Data;

namespace Core.SceneManagement.AppStateScenes.Contracts
{
    public interface ILoadingProgress
    {
        event Action<LoadingProgressInfo> Changed;

        LoadingProgressInfo Current { get; }

        void Reset(string message = "Loading...");
        void Report(float progress, string message = null);
    }
}