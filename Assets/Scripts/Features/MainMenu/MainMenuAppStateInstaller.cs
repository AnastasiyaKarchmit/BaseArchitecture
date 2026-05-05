using Core.AppStates.Components;
using Features.MainMenu.States.SettingsState;
using VContainer;

namespace Features.MainMenu
{
    public sealed class MainMenuAppStateInstaller : AppStateInstaller
    {
        public override void RegisterDependencies(IContainerBuilder builder)
        {
            builder.Register<MainMenuModel>(Lifetime.Singleton);
            builder.Register<MainMenuPresenter>(Lifetime.Singleton);
            builder.Register<SettingsPresenter>(Lifetime.Singleton);
            builder.Register<SettingsView>(Lifetime.Singleton);
            builder.Register<MainMenuFlowController>(Lifetime.Singleton);
            builder.Register<MainMenuAppStateController>(Lifetime.Singleton);
        }
    }
}