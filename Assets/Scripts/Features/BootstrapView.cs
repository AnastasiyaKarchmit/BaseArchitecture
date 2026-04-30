using Core.UI.Views;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Features
{
    public sealed class BootstrapView : BaseView
    {
        [SerializeField] private Slider progressBar;
        [SerializeField] private TMP_Text statusText;
        [SerializeField] private TMP_Text tipText;
        [SerializeField] private TMP_Text versionText;

        public void SetVersion(string version)
        {
            if (versionText != null)
                versionText.text = $"v{version}";
        }

        public void SetStatus(string status)
        {
            if (statusText != null)
                statusText.text = status;
        }

        public void SetProgress(float normalizedProgress)
        {
            if (progressBar != null)
                progressBar.value = Mathf.Clamp01(normalizedProgress);
        }

        public void SetTip(string tip)
        {
            if (tipText != null)
                tipText.text = tip;
        }

        public void SetLoadingCompleted()
        {
            SetProgress(1f);
            SetStatus("Ready");
        }
    }
}