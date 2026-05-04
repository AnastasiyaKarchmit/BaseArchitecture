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
        private readonly MainMenuPresenter _presenter;

        private readonly CompositeDisposable _disposables = new();
        private UniTaskCompletionSource<AppStateExitResult> _completionSource;

        public MainMenuAppStateController(MainMenuPresenter presenter)
        {
            _presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));
        }

        public async UniTask EnterAsync(object payload, CancellationToken token)
        {
            _completionSource = new UniTaskCompletionSource<AppStateExitResult>();

            _presenter.PlayRequested
                .Subscribe(_ =>
                {
                    _completionSource.TrySetResult(
                        AppStateExitResult.SwitchTo(AppStateId.Gameplay));
                })
                .AddTo(_disposables);

            await _presenter.EnterAsync(token);
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
            return _presenter.ExitAsync(token);
        }

        public void Dispose()
        {
            _disposables.Dispose();
            _presenter.Dispose();
        }
    }
}