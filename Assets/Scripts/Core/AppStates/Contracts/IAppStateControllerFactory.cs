using Core.AppStates.Contracts.State;
using Core.AppStates.Data;

namespace Core.AppStates.Contracts
{
    public interface IAppStateControllerFactory
    {
        IAppStateController Create(AppStateId stateId);
    }
}