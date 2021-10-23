using UnityEditor;
using UnityEngine;

namespace Editor
{
    /// <summary>
    /// ボールの更新
    /// </summary>
    public static class BallUpdater
    {
        private static Vector2 position = new Vector2(-100, -100);
        private static Vector2 moveDir = new Vector2(1, -1).normalized;

        private const float BALL_RADIUS = 7f;
        private const float MOVE_SPEED = 300f;

        public static void Initialize()
        {
            moveDir = new Vector2(1, -1).normalized;
        }

        /// <summary>
        /// ボールを毎フレーム更新
        /// </summary>
        public static void UpdateBall(Vector2 windowSize, float dt)
        {
            UpdateBallPos(windowSize, dt);
            DrawBall(position, BALL_RADIUS);

            if (GameStateManager.CurrentGameState == GameState.Playing && IsGameOver())
            {
                GameStateManager.OnGameOver();
            }
        }

        /// <summary>
        /// ボールの位置を更新
        /// </summary>
        private static void UpdateBallPos(Vector2 windowSize, float dt)
        {
            if (GameStateManager.CurrentGameState == GameState.Ready)
            {
                position = GetReadyBallPos();
            }
            else if (GameStateManager.CurrentGameState == GameState.Playing)
            {
                position = GetMovedBallPos(windowSize, dt);
            }
        }

        /// <summary>
        /// ゲーム開始前のボールの位置を取得(バーの中央)
        /// </summary>
        private static Vector2 GetReadyBallPos()
        {
            var barRect = BarUpdater.GetBarRect();
            var posX = barRect.center.x;
            var posY = barRect.yMin - BALL_RADIUS;
            return new Vector2(posX, posY);
        }

        /// <summary>
        /// ボールの移動
        /// </summary>
        private static Vector2 GetMovedBallPos(Vector2 windowSize, float dt)
        {
            var nextPos = position + moveDir * (MOVE_SPEED * dt);
            nextPos = BallCollisionCalculator.ReflectBar(position, nextPos, BALL_RADIUS, ref moveDir);
            nextPos = BallCollisionCalculator.ReflectBlocks(position, nextPos, windowSize, BALL_RADIUS, ref moveDir);
            nextPos = BallCollisionCalculator.ReflectWindowFrame(nextPos, windowSize, BALL_RADIUS, ref moveDir);
            
            return nextPos;
        }

        /// <summary>
        /// ボール（Toggle）を描画
        /// </summary>
        private static void DrawBall(Vector2 pos, float ballRadius)
        {
            GUI.color = Color.white;
            var rect = new Rect(pos.x - ballRadius, pos.y - ballRadius, ballRadius * 2, ballRadius * 2);
            EditorGUI.Toggle(rect, GameStateManager.CurrentGameState == GameState.Clear);
        }

        /// <summary>
        /// ゲームオーバー判定
        /// </summary>
        private static bool IsGameOver()
        {
            // バーより少し下に行ったらゲームオーバー
            return position.y > BarUpdater.GetBarRect().yMax + 40;
        }
    }
}