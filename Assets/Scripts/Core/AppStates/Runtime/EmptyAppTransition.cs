using System.Threading;
using Core.AppStates.Contracts;
using Cysharp.Threading.Tasks;

namespace Core.AppStates.Runtime
{
    public sealed class EmptyAppTransition : IAppTransition
    {
        public UniTask ShowAsync(CancellationToken token = default)
        {
            return UniTask.CompletedTask;
        }

        public UniTask HideAsync(CancellationToken token = default)
        {
            return UniTask.CompletedTask;
        }
    }
}