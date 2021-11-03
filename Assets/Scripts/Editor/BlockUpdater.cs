using UnityEditor;
using UnityEngine;

namespace Editor
{
    /// <summary>
    /// ブロックの更新
    /// [blockColorDataの要素数] x [blockColorDataの要素数] のブロックを配置する
    /// </summary>
    public static class BlockUpdater
    {
        private const int BLOCK_COUNT_ROW = 8;
        
        private static readonly Color[] blockColorData = new Color[BLOCK_COUNT_ROW]
        {
            new Color(1f, 0.4f, 0.3f),
            new Color(1f, 0.7f, 0.3f),
            new Color(1f, 0.9f, 0.3f),
            new Color(0.5f, 0.9f, 0.3f),
            new Color(0.3f, 0.9f, 0.7f),
            new Color(0.4f, 0.8f, 1f),
            new Color(0.6f, 0.5f, 0.9f),
            new Color(0.9f, 0.5f, 0.7f),
        };

        
        private static readonly bool[,] brokenStates = new bool[BLOCK_COUNT_ROW, BLOCK_COUNT_ROW];
        private static readonly float[,] animStates = new float[BLOCK_COUNT_ROW, BLOCK_COUNT_ROW];
        private static readonly Color[,] blockColors = new Color[BLOCK_COUNT_ROW, BLOCK_COUNT_ROW];

        private static GameState preGameState = GameState.Ready;
        
        private const float BROKEN_ANIM_DURATION = 0.3f;
        private const float GAME_OVER_ANIM_DURATION = 0.05f;

        public static void Initialize()
        {
            for (var y = 0; y < BLOCK_COUNT_ROW; y++)
            {
                for (var x = 0; x < BLOCK_COUNT_ROW; x++)
                {
                    brokenStates[x, y] = false;
                    animStates[x, y] = 0f;
                }
            }
        }

        #region Logic
        
        public static void UpdateBlocks(float dt)
        {
            UpdateBlockColors(dt);
        }

        /// <summary>
        /// ブロックの色を更新
        /// </summary>
        /// <param name="dt"></param>
        private static void UpdateBlockColors(float dt)
        {
            for (var y = 0; y < BLOCK_COUNT_ROW; y++)
            {
                for (var x = 0; x < BLOCK_COUNT_ROW; x++)
                {
                    blockColors[x, y] = GetBlockColor(x, y, dt);
                }
            }

            preGameState = GameStateManager.CurrentGameState;
        }

        /// <summary>
        /// ブロックの色を計算
        /// </summary>
        private static Color GetBlockColor(int x, int y, float dt)
        {
            var currentGameState = GameStateManager.CurrentGameState;
            switch (currentGameState)
            {
                case GameState.Clear:
                    if (currentGameState != preGameState) animStates[x, y] = x + y;

                    animStates[x, y] += dt * 5;
                    var isColorful = ((int) animStates[x, y]) % 3 == 0;
                    return isColorful ? GetDefaultColor(x, y) : new Color(1, 1, 1, 0);
                
                case GameState.GameOver:
                    if (currentGameState != preGameState) animStates[x, y] = 1f + y;

                    if (brokenStates[x, y]) return Color.clear;

                    animStates[x, y] = Mathf.Max(0, animStates[x, y] - dt / GAME_OVER_ANIM_DURATION);
                    return Color.Lerp(Color.white, GetDefaultColor(x, y), Mathf.Min(1, animStates[x, y]));
                
                default:
                    if (brokenStates[x, y])
                    {
                        animStates[x, y] = Mathf.Max(0, animStates[x, y] - dt / BROKEN_ANIM_DURATION);
                        return Color.Lerp(new Color(1, 1, 1, 0), Color.white, animStates[x, y]);
                    }

                    return GetDefaultColor(x, y);
            }
        }
        
        /// <summary>
        /// ブロックのデフォルト色を取得
        /// </summary>
        private static Color GetDefaultColor(int x, int y)
        {
            var colorIdx = (x + y) % blockColorData.Length;
            return blockColorData[colorIdx];
        }

        /// <summary>
        /// ブロックを崩す
        /// </summary>
        public static void Break(int x, int y)
        {
            brokenStates[x, y] = true;
            animStates[x, y] = 1f;

            if (IsBrokenAll())
            {
                GameStateManager.OnClear();
            }
        }

        /// <summary>
        /// 全てのブロックが崩されたか
        /// </summary>
        private static bool IsBrokenAll()
        {
            foreach (var brokenState in brokenStates)
            {
                if (!brokenState) return false;
            }

            return true;
        }

        /// <summary>
        /// ブロックが崩され済みか
        /// </summary>
        public static bool IsBroken(int x, int y)
        {
            return brokenStates[x, y];
        }

        /// <summary>
        /// ブロックのRect群を取得
        /// </summary>
        public static Rect[,] GetBlockRects(Vector2 windowSize)
        {
            var blockRects = new Rect[BLOCK_COUNT_ROW, BLOCK_COUNT_ROW];

            var paddingSide = LayoutManager.BLOCK_AREA_PADDING_SIDE;
            var paddingTop = LayoutManager.BLOCK_AREA_PADDING_TOP;
            var blockWidth = (windowSize.x - paddingSide * 2) / BLOCK_COUNT_ROW;
            var blockHeight = 21;

            for (var y = 0; y < BLOCK_COUNT_ROW; y++)
            {
                for (var x = 0; x < BLOCK_COUNT_ROW; x++)
                {
                    if (brokenStates[x, y]) continue;

                    var pos = new Vector2(paddingSide + blockWidth * x - 1, paddingTop + blockHeight * y + 1);
                    var size = new Vector2(blockWidth, blockHeight);
                    blockRects[x, y] = new Rect(pos, size);
                }
            }

            return blockRects;
        }

        #endregion

        #region Drawing
        
        /// <summary>
        /// 全ブロックの描画
        /// </summary>
        public static void DrawBlocks()
        {
            EditorGUILayout.Space(LayoutManager.BLOCK_AREA_PADDING_TOP);
            for (var y = 0; y < BLOCK_COUNT_ROW; y++)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(LayoutManager.BLOCK_AREA_PADDING_SIDE);

                for (var x = 0; x < BLOCK_COUNT_ROW; x++)
                {
                    DrawBlock(x, y);
                }

                GUILayout.Space(LayoutManager.BLOCK_AREA_PADDING_SIDE);
                EditorGUILayout.EndHorizontal();
            }
        }

        /// <summary>
        /// ブロックの描画
        /// </summary>
        private static void DrawBlock(int x, int y)
        {
            GUI.color = blockColors[x, y];
            GUILayout.Button("");
        }

        #endregion
    }
}