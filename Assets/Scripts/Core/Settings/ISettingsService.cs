using System;

namespace Core.Settings
{
    public interface ISettingsService
    {
        event Action<SettingsValues> Changed;

        float MusicVolume { get; }
        float SfxVolume { get; }

        SettingsValues GetValues();

        void SetMusicVolume(float value);
        void SetSfxVolume(float value);
        void ResetToDefaults();
    }
}