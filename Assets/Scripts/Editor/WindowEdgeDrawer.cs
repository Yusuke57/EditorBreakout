using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    /// <summary>
    /// ウィンドウ枠の描画
    /// </summary>
    public static class WindowEdgeDrawer
    {
        private static Dictionary<WindowEdge, float> edgeAnimStates = new Dictionary<WindowEdge, float>();
        
        private const float EDGE_ANIM_WIDTH = 1f;
        private const float EDGE_ANIM_DURATION = 0.3f;

        public static void Initialize()
        {
            edgeAnimStates = new Dictionary<WindowEdge, float>()
            {
                {WindowEdge.Top, 0},
                {WindowEdge.Right, 0},
                {WindowEdge.Bottom, 0},
                {WindowEdge.Left, 0},
            };

            var edges = edgeAnimStates.Keys.ToList();
            foreach (var edge in edges)
            {
                edgeAnimStates[edge] = 0f;
            }
        }
        
        /// <summary>
        /// ボール衝突時に呼び出される
        /// </summary>
        public static void OnCollisionBall(WindowEdge edge)
        {
            edgeAnimStates[edge] = 1f;
        }

        /// <summary>
        /// ウィンドウ枠の辺となるRectを取得
        /// </summary>
        private static Rect GetEdgeRect(WindowEdge edge, Vector2 windowSize)
        {
            switch (edge)
            {
                case WindowEdge.Top:
                    return new Rect(0, 0, windowSize.x, EDGE_ANIM_WIDTH);
                case WindowEdge.Bottom:
                    return new Rect(0, windowSize.y - EDGE_ANIM_WIDTH, windowSize.x, EDGE_ANIM_WIDTH);
                case WindowEdge.Left:
                    return new Rect(0, 0, EDGE_ANIM_WIDTH, windowSize.y);
                case WindowEdge.Right:
                    return new Rect(windowSize.x - EDGE_ANIM_WIDTH, 0, EDGE_ANIM_WIDTH, windowSize.y);
            }
            return Rect.zero;
        }

        /// <summary>
        /// ウィンドウ枠を表示
        /// </summary>
        public static void DrawWindowFrame(Vector2 windowSize, float dt)
        {
            var edges = edgeAnimStates.Keys.ToList();
            foreach (var edge in edges)
            {
                edgeAnimStates[edge] -= dt / EDGE_ANIM_DURATION;
                if (edgeAnimStates[edge] < 0) continue;

                var rect = GetEdgeRect(edge, windowSize);
                var color = new Color(0.7f, 0.7f, 0.7f, edgeAnimStates[edge]);
                EditorGUI.DrawRect(rect, color);
            }
        }

        public enum WindowEdge
        {
            Top = 0,
            Right = 1,
            Bottom = 2,
            Left = 3,
        }
    }
}