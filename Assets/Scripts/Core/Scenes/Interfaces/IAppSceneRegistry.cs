using System.Collections.Generic;
using Core.AppStates;

namespace Core.Scenes
{
    public interface IAppSceneRegistry
    {
        AppSceneSet GetSceneSet(AppStateId stateId);
        IReadOnlyList<string> GetPersistentScenes();
    }
}