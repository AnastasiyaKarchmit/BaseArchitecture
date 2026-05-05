using R3;

namespace Features.MainMenu.States.SettingsState
{
    public readonly struct SettingsViewCommands
    {
        public readonly ReactiveCommand<float> MusicVolumeChanged;
        public readonly ReactiveCommand<float> SfxVolumeChanged;
        public readonly ReactiveCommand<Unit> ResetClicked;
        public readonly ReactiveCommand<Unit> BackClicked;

        public SettingsViewCommands(
            ReactiveCommand<float> musicVolumeChanged,
            ReactiveCommand<float> sfxVolumeChanged,
            ReactiveCommand<Unit> resetClicked,
            ReactiveCommand<Unit> backClicked)
        {
            MusicVolumeChanged = musicVolumeChanged;
            SfxVolumeChanged = sfxVolumeChanged;
            ResetClicked = resetClicked;
            BackClicked = backClicked;
        }
    }
}