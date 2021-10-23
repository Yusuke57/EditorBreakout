namespace Editor
{
    public static class GameStateManager
    {
        public static GameState CurrentGameState = GameState.Ready;
        private static GameState pausedState = GameState.Ready;

        private static bool isPause;
        public static bool IsPause => isPause;

        public static void Initialize()
        {
            CurrentGameState = GameState.Ready;
            pausedState = GameState.Ready;
            isPause = false;
        }

        public static void Pause(GameState pausingState)
        {
            pausedState = CurrentGameState;
            CurrentGameState = pausingState;
            isPause = true;
        }

        public static void UnPause()
        {
            CurrentGameState = pausedState;
            isPause = false;
        }

        public static void OnPlay()
        {
            if (CurrentGameState != GameState.Ready) return;
            CurrentGameState = GameState.Playing;
            AudioPlayer.Play(AudioPlayer.ClipName.CollisionBar);
        }

        public static void OnClear()
        {
            if (CurrentGameState != GameState.Playing) return;
            CurrentGameState = GameState.Clear;
            AudioPlayer.Play(AudioPlayer.ClipName.GameClear);
        }

        public static void OnGameOver()
        {
            if (CurrentGameState != GameState.Playing) return;
            CurrentGameState = GameState.GameOver;
            AudioPlayer.Play(AudioPlayer.ClipName.GameOver);
        }
    }

    public enum GameState
    {
        Ready = 0,
        Playing,
        Clear,
        GameOver,
        LayoutError,
    }
}