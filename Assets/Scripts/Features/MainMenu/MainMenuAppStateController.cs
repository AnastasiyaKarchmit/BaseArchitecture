using System;
using System.Threading;
using Core.AppStates.Contracts.State;
using Core.AppStates.Data;
using Cysharp.Threading.Tasks;
using R3;

namespace Features.MainMenu
{
    public sealed class MainMenuAppStateController : IAppStateController
    {
        private readonly MainMenuFlowController _flowController;

        private readonly CompositeDisposable _disposables = new();
        private UniTaskCompletionSource<AppStateExitResult> _completionSource;

        public MainMenuAppStateController(MainMenuFlowController flowController)
        {
            _flowController = flowController ?? throw new ArgumentNullException(nameof(flowController));
        }

        public async UniTask EnterAsync(object payload, CancellationToken token)
        {
            _completionSource = new UniTaskCompletionSource<AppStateExitResult>();

            _flowController.PlayRequested
                .Subscribe(_ =>
                {
                    _completionSource.TrySetResult(
                        AppStateExitResult.SwitchTo(AppStateId.Gameplay));
                })
                .AddTo(_disposables);

            await _flowController.EnterAsync(token);
        }

        public async UniTask<AppStateExitResult> RunAsync(CancellationToken token)
        {
            await using var registration = token.Register(() =>
            {
                _completionSource.TrySetCanceled(token);
            });

            return await _completionSource.Task;
        }

        public UniTask ExitAsync(CancellationToken token)
        {
            _disposables.Clear();
            return _flowController.ExitAsync(token);
        }

        public void Dispose()
        {
            _disposables.Dispose();
            _flowController.Dispose();
        }
    }
}