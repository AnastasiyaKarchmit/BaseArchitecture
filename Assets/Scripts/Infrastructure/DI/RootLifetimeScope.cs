using Core.AppStates.Contracts;
using Core.AppStates.Runtime;
using Core.Input.Runtime;
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
using UnityEngine.InputSystem.UI;
using VContainer;
using VContainer.Unity;

namespace Infrastructure.DI
{
    public class RootLifetimeScope : LifetimeScope
    {
        [Header("References")]
        [SerializeField] private AppSceneDatabase appSceneDatabase;
        [SerializeField] private WindowServiceConfig windowServiceConfig;
        [SerializeField] private GameObject eventSystemPrefab;
        

        protected override void Configure(IContainerBuilder builder)
        {
            RegisterSceneManagement(builder);
            RegisterAppStateSystem(builder);
            RegisterConfigs(builder);
            RegisterServices(builder);
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
        
        private void RegisterInputService(IContainerBuilder builder)
        {
            GameObject eventSystemInstance = Instantiate(eventSystemPrefab);
            DontDestroyOnLoad(eventSystemInstance);
            InputSystemUIInputModule uiInputModule = eventSystemInstance.GetComponent<InputSystemUIInputModule>();
            builder.Register<InputService>(Lifetime.Singleton).AsImplementedInterfaces().WithParameter(uiInputModule);
        }

        private void RegisterConfigs(IContainerBuilder builder)
        {
            builder.RegisterInstance(windowServiceConfig);
        }
        
        private void RegisterServices(IContainerBuilder builder)
        {
            builder.Register<AddressableWindowFactory>(Lifetime.Scoped).AsImplementedInterfaces();
            RegisterInputService(builder);
        }
    }
}