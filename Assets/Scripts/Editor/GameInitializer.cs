using UnityEngine;

namespace Editor
{
    /// <summary>
    /// ゲームの初期化
    /// </summary>
    public static class GameInitializer
    {
        private const float INIT_BUTTON_HEIGHT = 20f;
        
        public static void Initialize()
        {
            GameStateManager.Initialize();
            AudioPlayer.Initialize();
            BlockUpdater.Initialize();
            BarUpdater.Initialize();
            BallUpdater.Initialize();
            WindowEdgeDrawer.Initialize();
        }

        public static void DrawInitializeButton(Vector2 windowSize)
        {
            GUI.backgroundColor = Color.white;
            if (GUI.Button(new Rect(0, windowSize.y - INIT_BUTTON_HEIGHT, windowSize.x, INIT_BUTTON_HEIGHT), "Reset"))
            {
                Initialize();
            }
        }

        public static void CheckPlayGame()
        {
            if (GameStateManager.CurrentGameState == GameState.Ready && Event.current.type == EventType.MouseUp)
            {
                GameStateManager.OnPlay();
            }
        }
    }
}