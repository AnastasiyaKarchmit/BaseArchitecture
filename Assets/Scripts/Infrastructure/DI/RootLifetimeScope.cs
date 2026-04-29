using Core.AppStates.Contracts;
using Core.AppStates.Runtime;
using Core.SceneManagement.AppStateScenes;
using Core.SceneManagement.AppStateScenes.Configs;
using Core.SceneManagement.AppStateScenes.Contracts;
using Core.SceneManagement.AppStateScenes.Runtime;
using Core.SceneManagement.Loading;
using Core.SceneManagement.Loading.Contracts;
using Core.SceneManagement.Loading.Runtime;
using Core.UI.Windows.Config;
using Core.UI.Windows.Runtime;
using Infrastructure.Factories;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Infrastructure.DI
{
    public class RootLifetimeScope : LifetimeScope
    {
        [Header("Scene Configuration")]
        [SerializeField] private AppSceneDatabase appSceneDatabase;
        [SerializeField] private WindowServiceConfig windowServiceConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            RegisterSceneManagement(builder);
            RegisterAppStateSystem(builder);
        }

        private void RegisterSceneManagement(IContainerBuilder builder)
        {
            builder.RegisterInstance(appSceneDatabase);

            builder.Register<ISceneLoadService, SceneLoadService>(Lifetime.Singleton);

            builder.Register<IAppSceneRegistry, AppSceneRegistry>(Lifetime.Singleton);
            builder.Register<IAppSceneCoordinator, AppSceneCoordinator>(Lifetime.Singleton);
        }

        private void RegisterAppStateSystem(IContainerBuilder builder)
        {
            builder.Register<IAppTransition, EmptyAppTransition>(Lifetime.Singleton);

            builder.Register<IAppStateControllerFactory, AppStateControllerFactory>(Lifetime.Singleton);

            builder.RegisterEntryPoint<AppStateMachine>();
        }

        private void RegisterConfigs(IContainerBuilder builder)
        {
            builder.RegisterInstance(windowServiceConfig);
        }
        
        private void RegisterWindowService(IContainerBuilder builder)
        {
            builder.Register<AddressableWindowFactory>(Lifetime.Scoped).AsImplementedInterfaces();
        }
    }
}