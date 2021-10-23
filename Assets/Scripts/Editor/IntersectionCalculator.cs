using UnityEngine;

namespace Editor
{
    /// <summary>
    /// 交点を計算する
    /// </summary>
    public static class IntersectionCalculator
    {
        /// <summary>
        /// 2つの線分が交差するか
        /// </summary>
        private static bool HasIntersection(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
        {
            var ta = (b1.x - b2.x) * (a1.y - b1.y) + (b1.y - b2.y) * (b1.x - a1.x);
            var tb = (b1.x - b2.x) * (a2.y - b1.y) + (b1.y - b2.y) * (b1.x - a2.x);
            var tc = (a1.x - a2.x) * (b1.y - a1.y) + (a1.y - a2.y) * (a1.x - b1.x);
            var td = (a1.x - a2.x) * (b2.y - a1.y) + (a1.y - a2.y) * (a1.x - b2.x);

            return tc * td <= 0 && ta * tb <= 0;
        }

        /// <summary>
        /// 矩形と線分の交点の方向を取得する
        /// </summary>
        public static Direction SearchIntersection(Rect rect, Vector2 a1, Vector2 a2)
        {
            var topLeft = new Vector2(rect.xMin, rect.yMin);
            var topRight = new Vector2(rect.xMax, rect.yMin);
            var bottomLeft = new Vector2(rect.xMin, rect.yMax);
            var bottomRight = new Vector2(rect.xMax, rect.yMax);
            
            if (HasIntersection(a1, a2, topLeft, bottomLeft)) return Direction.Left;
            if (HasIntersection(a1, a2, topLeft, topRight)) return Direction.Top;
            if (HasIntersection(a1, a2, bottomRight, topRight)) return Direction.Right;
            if (HasIntersection(a1, a2, bottomRight, bottomLeft)) return Direction.Bottom;
            return Direction.None;
        }
    }

    public enum Direction
    {
        None,
        Top,
        Bottom,
        Left,
        Right,
    }
}