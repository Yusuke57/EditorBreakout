using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    /// <summary>
    /// バーの更新
    /// </summary>
    public static class BarUpdater
    {
        private static float barLeftPos = -BAR_WIDTH / 2;
        private static float barRightPos = BAR_WIDTH / 2;
        private static float barPosY;
        private static float windowWidth;

        private static float mousePosX;
        
        private static Queue<float> barLeftPosQueue = new Queue<float>();
        private const int QUEUE_SIZE = 16;

        private const float BAR_WIDTH = 100f;
        private const float BAR_ROUND_SIZE = 6f;

        private static float collisionEffectState = 0;
        private const float COLLISION_EFFECT_DURATION = 0.2f;
        
        private static readonly Color barDefaultColor = new Color(0.7f, 0.7f, 0.7f);
        private static readonly Color barCollisionColor = Color.white;

        public static void Initialize()
        {
            barLeftPos = -BAR_WIDTH / 2;
            barRightPos = BAR_WIDTH / 2;
        }

        #region Logic
        
        public static void UpdateBar(Vector2 windowSize, float dt)
        {
            windowWidth = windowSize.x;
            
            MoveBar();

            barPosY = LayoutManager.GetBarPosY(windowSize);
            collisionEffectState = Mathf.Max(0, collisionEffectState - dt / COLLISION_EFFECT_DURATION);
        }

        /// <summary>
        /// バーの移動
        /// </summary>
        private static void MoveBar()
        {
            mousePosX = Mathf.Clamp(mousePosX, BAR_WIDTH / 2, windowWidth - BAR_WIDTH / 2);
            barLeftPos = mousePosX - BAR_WIDTH / 2 + BAR_ROUND_SIZE;
            barRightPos = mousePosX + BAR_WIDTH / 2 - BAR_ROUND_SIZE;

            if(barLeftPosQueue.Count >= QUEUE_SIZE) barLeftPosQueue.Dequeue();
            barLeftPosQueue.Enqueue(barLeftPos);
        }

        /// <summary>
        /// バーのRectを取得
        /// </summary>
        public static Rect GetBarRect()
        {
            var padding = 3f;
            return new Rect((barLeftPos + barRightPos) / 2 - BAR_WIDTH / 2 - padding, barPosY + 4, 
                BAR_WIDTH + padding * 2, 10 + padding);
        }

        /// <summary>
        /// ボールと衝突した時に呼び出される
        /// </summary>
        public static void OnCollisionBall()
        {
            collisionEffectState = 1;
        }

        /// <summary>
        /// バーの移動距離を取得
        /// </summary>
        public static float GetBarMoveDiff()
        {
            var posArray = barLeftPosQueue.ToArray();
            var diff = 0f;
            for (var i = 0; i < posArray.Length - 1; i++)
            {
                diff += posArray[i + 1] - posArray[i];
            }

            return diff / posArray.Length;
        }

        #endregion

        #region Drawing

        /// <summary>
        /// バーを描画
        /// </summary>
        public static void DrawBar()
        {
            GUI.color = Color.Lerp(barDefaultColor, barCollisionColor, collisionEffectState);

            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.MinMaxSlider(new Rect(-BAR_ROUND_SIZE, barPosY, windowWidth + BAR_ROUND_SIZE * 2, 10),
                new GUIContent(), ref barLeftPos, ref barRightPos, 0, windowWidth);
            EditorGUI.EndDisabledGroup();

            mousePosX = Event.current.mousePosition.x;
        }

        #endregion
    }
}