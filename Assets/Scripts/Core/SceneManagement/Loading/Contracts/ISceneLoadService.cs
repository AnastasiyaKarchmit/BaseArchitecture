using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Core.SceneManagement.Loading.Contracts
{
    public interface ISceneLoadService
    {
        UniTask LoadAdditiveAsync(string sceneName, CancellationToken token = default);
        UniTask LoadSingleAsync(string sceneName, CancellationToken token = default);
        UniTask UnloadAsync(string sceneName, CancellationToken token = default);
        bool IsLoaded(string sceneName);
        IReadOnlyList<string> GetLoadedSceneNames();
    }
}