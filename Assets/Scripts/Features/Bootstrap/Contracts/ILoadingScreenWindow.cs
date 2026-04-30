namespace Features.Bootstrap.Contracts
{
    public interface ILoadingScreenWindow
    {
        void SetProgress(float progress);
        void SetStatus(string status);
    }
}