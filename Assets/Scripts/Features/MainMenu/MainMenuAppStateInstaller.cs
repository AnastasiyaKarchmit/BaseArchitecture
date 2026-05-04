using Core.AppStates.Components;
using VContainer;

namespace Features.MainMenu
{
    public sealed class MainMenuAppStateInstaller : AppStateInstaller
    {
        public override void RegisterDependencies(IContainerBuilder builder)
        {
            builder.Register<MainMenuModel>(Lifetime.Singleton)
                .AsImplementedInterfaces()
                .AsSelf();
            builder.Register<MainMenuPresenter>(Lifetime.Singleton)
                .AsImplementedInterfaces()
                .AsSelf();
            builder.Register<MainMenuAppStateController>(Lifetime.Singleton)
                .AsImplementedInterfaces()
                .AsSelf();
        }
    }
}