using System;
using System.Collections.Generic;
using Core.AppStates;
using Core.Scenes.Configs;

namespace Core.Scenes
{
    public enum SharedSceneId
    {
        PopupLayer,
        LoadingScreen,
        DynamicBackground
    }

    public sealed class AppSceneRegistry : IAppSceneRegistry
    {
        private readonly AppSceneDatabase _database;

        public AppSceneRegistry(AppSceneDatabase database)
        {
            _database = database;
        }

        public IReadOnlyList<string> GetPersistentScenes()
        {
            return _database.PersistentScenes;
        }

        public AppSceneSet GetSceneSet(AppStateId stateId)
        {
            return _database.GetSceneSet(stateId);
        }
    }
}