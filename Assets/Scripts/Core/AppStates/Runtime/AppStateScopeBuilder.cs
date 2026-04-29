using System.Collections.Generic;
using System.Linq;
using Core.AppStates.Contracts;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace Core.AppStates.Runtime
{
    public interface IAppStateScopeBuilder
    {
        LifetimeScope BuildScope(
            LifetimeScope parentScope,
            IReadOnlyList<string> sceneNames,
            bool cleanupBeforeInstall = true);
    }

    public sealed class AppStateScopeBuilder : IAppStateScopeBuilder
    {
        public LifetimeScope BuildScope(
            LifetimeScope parentScope,
            IReadOnlyList<string> sceneNames,
            bool cleanupBeforeInstall = true)
        {
            var installers = FindInstallers(sceneNames);

            if (cleanupBeforeInstall)
            {
                foreach (var installer in installers)
                    installer.CleanupBeforeInstall();
            }

            var stateScope = parentScope.CreateChild(builder =>
            {
                foreach (var installer in installers)
                    installer.RegisterDependencies(builder);
            });

            // foreach (var installer in installers)
            //     installer.InjectSceneObjects(stateScope.Container);

            return stateScope;
        }

        private static IReadOnlyList<IAppStateInstaller> FindInstallers(
            IReadOnlyList<string> sceneNames)
        {
            var result = new List<IAppStateInstaller>();

            foreach (var sceneName in sceneNames)
            {
                var scene = SceneManager.GetSceneByName(sceneName);

                if (!scene.IsValid() || !scene.isLoaded)
                {
                    Debug.LogWarning($"Scene '{sceneName}' is not loaded. Cannot search app state installers.");
                    continue;
                }

                foreach (var rootObject in scene.GetRootGameObjects())
                {
                    var installers = rootObject.GetComponentsInChildren<IAppStateInstaller>(true);
                    result.AddRange(installers);
                }
            }

            return result
                .Distinct()
                .ToArray();
        }
    }
}