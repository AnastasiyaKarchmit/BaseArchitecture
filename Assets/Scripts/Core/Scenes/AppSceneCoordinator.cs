using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Core.AppStates;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.Scenes
{
    public class AppSceneCoordinator : IAppSceneCoordinator
    {
        private readonly ISceneLoadService _sceneLoader;
        private readonly IAppSceneRegistry _sceneRegistry;

        private readonly HashSet<string> _persistentScenes = new();
        private readonly HashSet<string> _currentStateScenes = new();

        public IReadOnlyList<string> CurrentStateScenes => _currentStateScenes.ToArray();

        public AppSceneCoordinator(
            ISceneLoadService sceneLoader,
            IAppSceneRegistry sceneRegistry)
        {
            _sceneLoader = sceneLoader;
            _sceneRegistry = sceneRegistry;
        }

        public async UniTask InitializePersistentScenesAsync(CancellationToken token = default)
        {
            var scenes = _sceneRegistry.GetPersistentScenes();

            foreach (var scene in scenes)
            {
                _persistentScenes.Add(scene);
                await _sceneLoader.LoadAdditiveAsync(scene, token);
            }
        }

        public async UniTask LoadStateScenesAsync(AppStateId stateId, CancellationToken token = default)
        {
            var sceneSet = _sceneRegistry.GetSceneSet(stateId);
            var requiredScenes = sceneSet.AllScenes.ToHashSet();

            await LoadScenesAsync(requiredScenes, token);
            await UnloadUnusedScenesAsync(requiredScenes, token);

            SetActiveScene(sceneSet.MainScene);

            _currentStateScenes.Clear();

            foreach (var scene in requiredScenes)
                _currentStateScenes.Add(scene);
        }

        private async UniTask LoadScenesAsync(
            IReadOnlyCollection<string> requiredScenes,
            CancellationToken token)
        {
            var tasks = requiredScenes
                .Where(scene => !_sceneLoader.IsLoaded(scene))
                .Select(scene => _sceneLoader.LoadAdditiveAsync(scene, token));

            await UniTask.WhenAll(tasks);
        }

        private async UniTask UnloadUnusedScenesAsync(
            IReadOnlyCollection<string> requiredScenes,
            CancellationToken token)
        {
            var loadedScenes = _sceneLoader.GetLoadedSceneNames();

            var scenesToUnload = loadedScenes
                .Where(scene => !_persistentScenes.Contains(scene))
                .Where(scene => !requiredScenes.Contains(scene))
                .ToArray();

            var tasks = scenesToUnload
                .Select(scene => _sceneLoader.UnloadAsync(scene, token));

            await UniTask.WhenAll(tasks);
        }

        private static void SetActiveScene(string sceneName)
        {
            var scene = SceneManager.GetSceneByName(sceneName);

            if (!scene.IsValid() || !scene.isLoaded)
            {
                Debug.LogError($"Cannot set active scene. Scene '{sceneName}' is not loaded.");
                return;
            }

            SceneManager.SetActiveScene(scene);
        }
    }
}