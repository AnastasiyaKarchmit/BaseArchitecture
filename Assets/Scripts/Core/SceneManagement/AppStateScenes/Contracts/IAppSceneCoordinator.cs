using System.Collections.Generic;
using System.Threading;
using Core.AppStates.Data;
using Cysharp.Threading.Tasks;

namespace Core.SceneManagement.AppStateScenes.Contracts
{
    public interface IAppSceneCoordinator
    {
        IReadOnlyList<string> CurrentStateScenes { get; }

        UniTask InitializePersistentScenesAsync(CancellationToken token = default);
        UniTask LoadStateScenesAsync(AppStateId stateId, CancellationToken token = default);
    }
}