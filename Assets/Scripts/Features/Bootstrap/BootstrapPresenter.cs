using System;
using System.Threading;
using Core.Input.Contracts;
using Core.Input.Runtime;
using Core.Patterns.MVP;
using Core.UI.Windows.Contracts;
using Core.UI.Windows.Data;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Features.Bootstrap
{
   public sealed class BootstrapPresenter : IPresenter
    {
        private readonly BootstrapModel _model;
        private readonly IWindowService _windowService;
        private readonly IInputService _inputService;

        private BootstrapView _view;
        private CancellationTokenSource _tipLoopCts;

        public BootstrapPresenter(
            BootstrapModel model,
            IWindowService windowService,
            IInputService inputService)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
            _windowService = windowService ?? throw new ArgumentNullException(nameof(windowService));
            _inputService = inputService ?? throw new ArgumentNullException(nameof(inputService));
        }

        public async UniTask EnterAsync(CancellationToken token = default)
        {
            _inputService.SetMode(InputMode.Disabled);

            _view = await _windowService.GetOrCreateAsync<BootstrapView>(
                WindowId.LoadingScreen,
                token);

            _view.SetVersion(Application.version);
            _view.SetProgress(0f);
            _view.SetStatus("Starting...");

            await _view.ShowAsync();

            _tipLoopCts = CancellationTokenSource.CreateLinkedTokenSource(token);
            RunTipLoopAsync(_tipLoopCts.Token).Forget();
        }

        public async UniTask RunAsync(CancellationToken token)
        {
            var progress = new Progress<float>(value =>
            {
                _view?.SetProgress(value);
            });

            await _model.RunStartupTasksAsync(
                progress,
                status => _view?.SetStatus(status),
                token);

            _view?.SetLoadingCompleted();

            await UniTask.Delay(500, cancellationToken: token);
        }

        public async UniTask ExitAsync(CancellationToken token = default)
        {
            StopTipLoop();

            if (_view != null)
                await _view.HideAsync();
        }

        private async UniTaskVoid RunTipLoopAsync(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    _view?.SetTip(_model.GetNextTip());

                    await UniTask.Delay(
                        _model.TooltipDelayMs,
                        cancellationToken: token);
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        private void StopTipLoop()
        {
            if (_tipLoopCts == null)
                return;

            _tipLoopCts.Cancel();
            _tipLoopCts.Dispose();
            _tipLoopCts = null;
        }

        public void Dispose()
        {
            StopTipLoop();
        }
    }
}