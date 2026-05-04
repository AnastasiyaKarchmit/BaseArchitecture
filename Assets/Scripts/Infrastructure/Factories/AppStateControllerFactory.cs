using System;
using Core.AppStates.Contracts;
using Core.AppStates.Contracts.State;
using Core.AppStates.Data;
using Features.Bootstrap;
using Features.MainMenu;
using VContainer;

namespace Infrastructure.Factories
{
    public sealed class AppStateControllerFactory : IAppStateControllerFactory
    {
        public IAppStateController Create(AppStateId stateId, IObjectResolver resolver)
        {
            return stateId switch
            {
                AppStateId.Bootstrap => resolver.Resolve<BootstrapAppStateController>(),
                AppStateId.MainMenu => resolver.Resolve<MainMenuAppStateController>(),

                _ => throw new ArgumentOutOfRangeException(nameof(stateId), stateId, null)
            };
        }
    }
}