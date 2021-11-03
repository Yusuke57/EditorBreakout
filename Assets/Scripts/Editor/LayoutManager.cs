using UnityEngine;

namespace Editor
{
    /// <summary>
    /// レイアウト制御
    /// </summary>
    public static class LayoutManager
    {
        // window
        private const float WINDOW_MIN_WIDTH = 200;
        private const float WINDOW_MIN_HEIGHT = 360;
        
        // block area
        public const float BLOCK_AREA_PADDING_TOP = 40;
        public const float BLOCK_AREA_PADDING_SIDE = 40;

        // bar
        private const float BAR_MIN_POS_Y = 200;
        private const float BAR_PREFERRED_POS_Y = 400;
        
        // bottom
        private const float BOTTOM_PADDING = 40;

        /// <summary>
        /// ウィンドウサイズが適切かをチェックする
        /// </summary>
        public static bool CheckWindowSize(Rect windowArea)
        {
            var isValid = IsValidWindow(windowArea);
            if (isValid && GameStateManager.CurrentGameState == GameState.LayoutError)
            {
                GameStateManager.UnPause();
            }
            else if (!isValid && GameStateManager.CurrentGameState != GameState.LayoutError)
            {
                GameStateManager.Pause(GameState.LayoutError);
            }

            return isValid;
        }

        /// <summary>
        /// 適切なウィンドウサイズか
        /// </summary>
        private static bool IsValidWindow(Rect windowArea)
        {
            if (windowArea.width < WINDOW_MIN_WIDTH) return false;
            if (windowArea.height < WINDOW_MIN_HEIGHT) return false;

            return true;
        }
        
        /// <summary>
        /// バーのy座標を計算
        /// </summary>
        public static float GetBarPosY(Vector2 windowSize)
        {
            if (windowSize.y - BOTTOM_PADDING > BAR_PREFERRED_POS_Y) return BAR_PREFERRED_POS_Y;
            return Mathf.Max(BAR_MIN_POS_Y, windowSize.y - BOTTOM_PADDING);
        }
    }
}