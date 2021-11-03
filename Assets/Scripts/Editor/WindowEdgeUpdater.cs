using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    /// <summary>
    /// ウィンドウ枠の描画
    /// </summary>
    public static class WindowEdgeUpdater
    {
        private static Dictionary<WindowEdge, float> edgeAnimStates = new Dictionary<WindowEdge, float>()
        {
            {WindowEdge.Top, 0},
            {WindowEdge.Right, 0},
            {WindowEdge.Bottom, 0},
            {WindowEdge.Left, 0},
        };
        private static Dictionary<Rect, Color> edgeDrawDataList = new Dictionary<Rect, Color>();
        
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
            
            edgeDrawDataList = new Dictionary<Rect, Color>();
        }

        /// <summary>
        /// 枠を更新
        /// </summary>
        public static void UpdateWindowEdges(Vector2 windowSize, float dt)
        {
            edgeDrawDataList.Clear();
            
            var edges = new List<WindowEdge>(edgeAnimStates.Keys);
            foreach (var edge in edges)
            {
                edgeAnimStates[edge] -= dt / EDGE_ANIM_DURATION;
                if(edgeAnimStates[edge] <= 0) continue;

                var rect = GetEdgeRect(edge, windowSize);
                var alpha = Mathf.Clamp01(edgeAnimStates[edge]);
                var color = new Color(0.5f, 0.5f, 0.5f, alpha);
                edgeDrawDataList.Add(rect, color);
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
        public static void DrawWindowFrame()
        {
            foreach (var edgeDrawData in edgeDrawDataList)
            {
                EditorGUI.DrawRect(edgeDrawData.Key, edgeDrawData.Value);
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