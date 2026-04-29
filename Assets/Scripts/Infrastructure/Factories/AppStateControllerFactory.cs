using System;
using Core.AppStates.Contracts;
using Core.AppStates.Contracts.State;
using Core.AppStates.Data;

namespace Infrastructure.Factories
{
    public sealed class AppStateControllerFactory : IAppStateControllerFactory
    {
        public IAppStateController Create(AppStateId stateId)
        {
            throw new NotImplementedException(
                $"App state factory is not configured for state '{stateId}' yet.");
        }
    }
}