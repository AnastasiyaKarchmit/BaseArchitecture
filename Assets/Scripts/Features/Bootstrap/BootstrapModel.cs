using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Features.Bootstrap.Startup;

namespace Features.Bootstrap
{
    public sealed class BootstrapModel
    {
        private readonly StartupTaskRunner _startupTaskRunner;

        private readonly string[] _tips =
        {
            "Use WASD or left stick to move.",
            "You can change settings from the pause menu.",
            "Addressables are used for loading UI windows.",
            "Progress is saved automatically."
        };

        private int _tipIndex;

        public int TooltipDelayMs => 3000;
        public int TargetFrameRate => 60;

        public BootstrapModel(StartupTaskRunner startupTaskRunner)
        {
            _startupTaskRunner = startupTaskRunner;
        }

        public string GetNextTip()
        {
            if (_tips.Length == 0)
                return string.Empty;

            var tip = _tips[_tipIndex];
            _tipIndex = (_tipIndex + 1) % _tips.Length;

            return tip;
        }

        public UniTask RunStartupTasksAsync(
            IProgress<float> progress,
            Action<string> statusChanged,
            CancellationToken token)
        {
            return _startupTaskRunner.RunAsync(progress, statusChanged, token);
        }
    }
}