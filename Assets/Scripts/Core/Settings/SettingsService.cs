using System;
using Core.Save;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Settings
{
    public sealed class SettingsService : ISettingsService, ISaveDataProvider, IDisposable
    {
        private const float DefaultMusicVolume = 1f;
        private const float DefaultSfxVolume = 1f;

        private readonly ISaveSystem _saveSystem;

        private float _musicVolume = DefaultMusicVolume;
        private float _sfxVolume = DefaultSfxVolume;

        public event Action<SettingsValues> Changed;

        public float MusicVolume => _musicVolume;
        public float SfxVolume => _sfxVolume;

        public SettingsService(ISaveSystem saveSystem)
        {
            _saveSystem = saveSystem ?? throw new ArgumentNullException(nameof(saveSystem));
            _saveSystem.Register(this);
        }

        public UniTask LoadAsync(PersistentData data)
        {
            data.Settings ??= new SettingsData();

            _musicVolume = Mathf.Clamp01(data.Settings.MusicVolume);
            _sfxVolume = Mathf.Clamp01(data.Settings.SfxVolume);
            
            ApplyRuntimeSettings();
            NotifyChanged();

            return UniTask.CompletedTask;
        }

        public void Save(PersistentData data)
        {
            data.Settings ??= new SettingsData();

            data.Settings.MusicVolume = _musicVolume;
            data.Settings.SfxVolume = _sfxVolume;
        }

        public SettingsValues GetValues()
        {
            return new SettingsValues(_musicVolume, _sfxVolume);
        }

        public void SetMusicVolume(float value)
        {
            value = Mathf.Clamp01(value);

            if (Mathf.Approximately(_musicVolume, value))
                return;

            _musicVolume = value;

            ApplyRuntimeSettings();
            NotifyChanged();
        }

        public void SetSfxVolume(float value)
        {
            value = Mathf.Clamp01(value);

            if (Mathf.Approximately(_sfxVolume, value))
                return;

            _sfxVolume = value;

            ApplyRuntimeSettings();
            NotifyChanged();
        }

        public void ResetToDefaults()
        {
            _musicVolume = DefaultMusicVolume;
            _sfxVolume = DefaultSfxVolume;

            ApplyRuntimeSettings();
            NotifyChanged();
        }

        private void ApplyRuntimeSettings()
        {
            // Temporary simple example.
            // Later this can call AudioService instead.
            AudioListener.volume = _musicVolume;

            // SfxVolume is stored and exposed.
            // Later you can pass it to AudioService / AudioMixer.
        }

        private void NotifyChanged()
        {
            Changed?.Invoke(GetValues());
        }

        public void Dispose()
        {
            _saveSystem.Unregister(this);
        }
    }
}