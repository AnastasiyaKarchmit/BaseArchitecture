namespace Core.Settings
{
    public readonly struct SettingsValues
    {
        public readonly float MusicVolume;
        public readonly float SfxVolume;

        public SettingsValues(float musicVolume, float sfxVolume)
        {
            MusicVolume = musicVolume;
            SfxVolume = sfxVolume;
        }
    }
}