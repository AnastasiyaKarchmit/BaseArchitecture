using System;
using Cysharp.Threading.Tasks;

namespace Core.Patterns.MVP
{
    public interface IPresenter : IDisposable
    {
        UniTask Enter(object param);
        UniTask Exit();
    }
}