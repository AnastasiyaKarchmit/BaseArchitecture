using System;
using System.Collections.Generic;
using System.Linq;
using Core.Application;
using Core.Save.SaveStorage;
using Cysharp.Threading.Tasks;
using VContainer.Unity;

namespace Core.Save
{
    public sealed class SaveSystem : IStartable, IDisposable
    {
        private const string SaveFileName = "save.json";

        private readonly ISaveStorage _storage;
        private readonly IAppLifecycleService _appLifecycleService;
        private readonly List<ISaveDataProvider> _providers;

        private PersistentData _data;
        private bool _isLoaded;
        private bool _isSaving;

        public PersistentData Data => _data;
        public bool IsLoaded => _isLoaded;

        public SaveSystem(
            ISaveStorage storage,
            IAppLifecycleService appLifecycleService,
            IReadOnlyList<ISaveDataProvider> providers)
        {
            _storage = storage;
            _appLifecycleService = appLifecycleService;
            _providers = providers.ToList();
        }

        public void Start()
        {
            _appLifecycleService.ApplicationFocusChanged += OnApplicationFocusChanged;
            _appLifecycleService.ApplicationPauseChanged += OnApplicationPauseChanged;
            _appLifecycleService.ApplicationQuitRequested += OnApplicationQuitRequested;

            LoadAsync().Forget();
        }

        public async UniTask LoadAsync()
        {
            if (_isLoaded)
                return;

            _data = await _storage.LoadAsync(SaveFileName, new PersistentData());

            foreach (ISaveDataProvider provider in _providers)
                await provider.LoadAsync(_data);

            _isLoaded = true;
        }

        public void RegisterRuntimeProvider(ISaveDataProvider provider)
        {
            if (_providers.Contains(provider))
                return;

            _providers.Add(provider);

            if (_isLoaded)
                provider.LoadAsync(_data).Forget();
        }

        public void UnregisterRuntimeProvider(ISaveDataProvider provider)
        {
            _providers.Remove(provider);
        }

        public async UniTask SaveAsync()
        {
            if (!_isLoaded)
                return;

            if (_isSaving)
                return;

            _isSaving = true;

            try
            {
                foreach (ISaveDataProvider provider in _providers)
                    provider.Save(_data);

                await _storage.SaveAsync(SaveFileName, _data);
            }
            finally
            {
                _isSaving = false;
            }
        }

        public async UniTask ResetAsync()
        {
            _data = new PersistentData();

            foreach (ISaveDataProvider provider in _providers)
                await provider.LoadAsync(_data);

            await SaveAsync();
        }

        private void OnApplicationFocusChanged(bool isFocused)
        {
            if (!isFocused)
                SaveAsync().Forget();
        }

        private void OnApplicationPauseChanged(bool isPaused)
        {
            if (isPaused)
                SaveAsync().Forget();
        }

        private void OnApplicationQuitRequested()
        {
            SaveAsync().Forget();
        }

        public void Dispose()
        {
            _appLifecycleService.ApplicationFocusChanged -= OnApplicationFocusChanged;
            _appLifecycleService.ApplicationPauseChanged -= OnApplicationPauseChanged;
            _appLifecycleService.ApplicationQuitRequested -= OnApplicationQuitRequested;
        }
    }
}