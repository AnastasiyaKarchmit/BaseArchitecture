using System.Collections.Generic;
using System.Threading;
using Core.AppStates;
using Cysharp.Threading.Tasks;

namespace Core.Scenes
{
    public interface IAppSceneCoordinator
    {
        IReadOnlyList<string> CurrentStateScenes { get; }

        UniTask InitializePersistentScenesAsync(CancellationToken token = default);
        UniTask LoadStateScenesAsync(AppStateId stateId, CancellationToken token = default);
    }
}