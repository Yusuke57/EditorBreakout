using UnityEditor;
using UnityEngine;

namespace Editor
{
    /// <summary>
    /// メッセージ表示
    /// </summary>
    public static class MessageShower
    {
        /// <summary>
        /// 中央（ブロック下）にメッセージ表示
        /// </summary>
        public static void ShowCenterMessage()
        {
            switch (GameStateManager.CurrentGameState)
            {
                case GameState.Ready:
                    EditorGUILayout.HelpBox("Click to Start", MessageType.Info);
                    break;
                case GameState.Clear:
                    GUI.backgroundColor = new Color(0.4f, 1f, 0.2f);
                    EditorGUILayout.HelpBox("Game Clear!", MessageType.Info);
                    break;
                case GameState.GameOver:
                    GUI.backgroundColor = new Color(0.3f, 0.1f, 0.1f);
                    EditorGUILayout.HelpBox("Game Over...", MessageType.Error);
                    break;
            }
        }

        /// <summary>
        /// ウィンドウ最上部にメッセージ表示
        /// </summary>
        public static void ShowTopMessage()
        {
            if (GameStateManager.CurrentGameState == GameState.LayoutError)
            {
                GUI.backgroundColor = new Color(0.5f, 0.1f, 0.1f);
                EditorGUILayout.HelpBox("Window size is too small.", MessageType.Error);   
            }
        }
    }
}