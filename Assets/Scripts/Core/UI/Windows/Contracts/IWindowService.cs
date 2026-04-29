using System;
using System.Threading;
using Core.UI.Windows.Data;
using Cysharp.Threading.Tasks;

namespace Core.UI.Windows.Contracts
{
    public interface IWindowService
    {
        event Action OnBeforeFirstWindowCreated;
        event Action OnBecameEmpty;

        bool IsLoadingAnyWindow { get; }

        UniTask<IWindow> CreateAsync(WindowId windowId, CancellationToken token = default);
        UniTask<IWindow> GetOrCreateAsync(WindowId windowId, CancellationToken token = default);

        bool TryFind(WindowId windowId, out IWindow window);
        IWindow GetTopWindow();

        void DestroyAll();
    }
}