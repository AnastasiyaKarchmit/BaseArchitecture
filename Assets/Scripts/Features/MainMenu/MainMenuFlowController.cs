using System;
using System.Threading;
using Core.Input.Runtime;
using Core.UI.Windows.Contracts;
using Cysharp.Threading.Tasks;
using Features.MainMenu.States.MainMenuState;
using Features.Shared.SettingsState;
using R3;
using Stateless;
using UnityEngine;

namespace Features.MainMenu
{
    public enum MainMenuFlowState
    {
        Main,
        Settings
    }

    public enum MainMenuFlowTrigger
    {
        OpenSettings,
        BackToMain
    }
    
    public sealed class MainMenuFlowController : IDisposable
    {
        private readonly MainMenuPresenter _mainMenuPresenter;
        private readonly SettingsPresenter _settingsPresenter;
        private readonly InputGate _inputGate;
        private readonly IWindowTransitionBackground _windowTransitionBackground;

        private readonly StateMachine<MainMenuFlowState, MainMenuFlowTrigger> _stateMachine;
        private readonly CompositeDisposable _disposables = new();
        private readonly ReactiveCommand<Unit> _playRequested = new();

        private CancellationToken _currentToken;
        
        private const float InputUnlockDelay = 0.2f;
        
        private bool _isTransitioning;

        public Observable<Unit> PlayRequested => _playRequested;

        public MainMenuFlowController(
            MainMenuPresenter mainMenuPresenter,
            SettingsPresenter settingsPresenter,
            InputGate inputGate, IWindowTransitionBackground windowTransitionBackground)
        {
            _mainMenuPresenter = mainMenuPresenter ?? throw new ArgumentNullException(nameof(mainMenuPresenter));
            _settingsPresenter = settingsPresenter ?? throw new ArgumentNullException(nameof(settingsPresenter));
            _inputGate = inputGate ?? throw new ArgumentNullException(nameof(inputGate));
            _windowTransitionBackground = windowTransitionBackground 
                                          ?? throw new ArgumentNullException(nameof(windowTransitionBackground));

            _stateMachine = new StateMachine<MainMenuFlowState, MainMenuFlowTrigger>(
                MainMenuFlowState.Main);

            ConfigureStateMachine();
            SubscribeToPresenterEvents();
        }

        public async UniTask EnterAsync(CancellationToken token)
        {
            _currentToken = token;
            
            _mainMenuPresenter.HideInstantly();
            _settingsPresenter.HideInstantly();

            await _mainMenuPresenter.EnterAsync(token);
        }

        public async UniTask ExitAsync(CancellationToken token)
        {
            await UniTask.WhenAll(
                _mainMenuPresenter.ExitAsync(token),
                _settingsPresenter.ExitAsync(token));
        }

        private void ConfigureStateMachine()
        {
            _stateMachine.Configure(MainMenuFlowState.Main)
                .OnEntryAsync(async () =>
                {
                    await UniTask.SwitchToMainThread();
                    await _mainMenuPresenter.EnterAsync(_currentToken);
                })
                .OnExitAsync(async () =>
                {
                    await UniTask.SwitchToMainThread();
                    _windowTransitionBackground.Create();
                    await _mainMenuPresenter.ExitAsync(_currentToken);
                })
                .Permit(MainMenuFlowTrigger.OpenSettings, MainMenuFlowState.Settings)
                .Ignore(MainMenuFlowTrigger.BackToMain);

            _stateMachine.Configure(MainMenuFlowState.Settings)
                .OnEntryAsync(async () =>
                {
                    await UniTask.SwitchToMainThread();
                    await _settingsPresenter.EnterAsync(_currentToken);
                })
                .OnExitAsync(async () =>
                {
                    await UniTask.SwitchToMainThread();
                    await _settingsPresenter.ExitAsync(_currentToken);
                })
                .Permit(MainMenuFlowTrigger.BackToMain, MainMenuFlowState.Main)
                .Ignore(MainMenuFlowTrigger.OpenSettings);

            _stateMachine.OnTransitionCompleted(transition =>
            {
                Debug.Log($"MainMenu flow: {transition.Source} -> {transition.Destination} by {transition.Trigger}");
            });
        }

        private void SubscribeToPresenterEvents()
        {
            _mainMenuPresenter.PlayRequested
                .Subscribe(_ => _playRequested.Execute(Unit.Default))
                .AddTo(_disposables);

            _mainMenuPresenter.SettingsRequested
                .SubscribeAwait(
                    async (_, token) => await FireAsync(MainMenuFlowTrigger.OpenSettings),
                    AwaitOperation.Drop)
                .AddTo(_disposables);

            _settingsPresenter.BackRequested
                .SubscribeAwait(
                    async (_, token) => await FireAsync(MainMenuFlowTrigger.BackToMain),
                    AwaitOperation.Drop)
                .AddTo(_disposables);
        }

        private async UniTask FireAsync(MainMenuFlowTrigger trigger)
        {
            if (_isTransitioning)
                return;

            if (!_inputGate.CanReceiveInput)
                return;

            if (!_stateMachine.CanFire(trigger))
            {
                Debug.LogWarning($"Cannot fire {trigger} from {_stateMachine.State}");
                return;
            }

            _isTransitioning = true;
            _inputGate.Block();

            try
            {
                await _stateMachine.FireAsync(trigger);
            }
            finally
            {
                _isTransitioning = false;

                _inputGate.Unblock();
                _inputGate.BlockFor(InputUnlockDelay);
            }
        }

        public void Dispose()
        {
            _disposables.Dispose();
            _playRequested.Dispose();

            _mainMenuPresenter.Dispose();
            _settingsPresenter.Dispose();
        }
    }
}