namespace Core.AppStates.Data
{
    public readonly struct AppStateExitResult
    {
        public AppStateId? NextState { get; }
        public object Payload { get; }

        public bool HasNextState => NextState.HasValue;

        private AppStateExitResult(AppStateId? nextState, object payload)
        {
            NextState = nextState;
            Payload = payload;
        }

        public static AppStateExitResult None => new(null, null);

        public static AppStateExitResult SwitchTo(AppStateId stateId, object payload = null)
        {
            return new AppStateExitResult(stateId, payload);
        }
    }
}