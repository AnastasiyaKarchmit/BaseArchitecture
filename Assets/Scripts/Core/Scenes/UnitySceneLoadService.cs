using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Core.Scenes
{
    public class UnitySceneLoadService : ISceneLoadService
    {
        public async UniTask LoadAdditiveAsync(string sceneName, CancellationToken token = default)
        {
            if (IsLoaded(sceneName))
                return;

            await LoadAsync(sceneName, LoadSceneMode.Additive, token);
        }

        public async UniTask LoadSingleAsync(string sceneName, CancellationToken token = default)
        {
            await LoadAsync(sceneName, LoadSceneMode.Single, token);
        }

        public async UniTask UnloadAsync(string sceneName, CancellationToken token = default)
        {
            var scene = SceneManager.GetSceneByName(sceneName);

            if (!scene.IsValid() || !scene.isLoaded)
                return;

            var operation = SceneManager.UnloadSceneAsync(scene);

            if (operation == null)
                return;

            await operation.ToUniTask(cancellationToken: token);
        }

        public bool IsLoaded(string sceneName)
        {
            var scene = SceneManager.GetSceneByName(sceneName);
            return scene.IsValid() && scene.isLoaded;
        }

        public IReadOnlyList<string> GetLoadedSceneNames()
        {
            var result = new List<string>();

            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);

                if (scene.IsValid() && scene.isLoaded)
                    result.Add(scene.name);
            }

            return result;
        }

        private static async UniTask LoadAsync(
            string sceneName,
            LoadSceneMode mode,
            CancellationToken token)
        {
            var operation = SceneManager.LoadSceneAsync(sceneName, mode);

            if (operation == null)
                throw new InvalidOperationException($"Cannot load scene '{sceneName}'. Check Build Settings.");

            await operation.ToUniTask(cancellationToken: token);
        }
    }
}