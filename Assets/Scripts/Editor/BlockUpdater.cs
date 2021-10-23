using UnityEditor;
using UnityEngine;

namespace Editor
{
    /// <summary>
    /// ブロックの更新
    /// </summary>
    public static class BlockUpdater
    {
        private static readonly Color[] blockColors = new Color[]
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

        private static readonly bool[,] brokenStates = new bool[blockColors.Length, blockColors.Length];
        private static readonly float[,] animStates = new float[blockColors.Length, blockColors.Length];

        private static GameState preGameState = GameState.Ready;
        
        private const float BROKEN_ANIM_DURATION = 0.3f;
        private const float GAME_OVER_ANIM_DURATION = 0.05f;

        public static void Initialize()
        {
            var blockColorCount = blockColors.Length;
            for (var y = 0; y < blockColorCount; y++)
            {
                for (var x = 0; x < blockColorCount; x++)
                {
                    brokenStates[x, y] = false;
                    animStates[x, y] = 0f;
                }
            }
        }

        public static void UpdateBlocks(float dt)
        {
            DrawBlocks(dt);
            preGameState = GameStateManager.CurrentGameState;
        }

        /// <summary>
        /// 全ブロックの描画
        /// </summary>
        private static void DrawBlocks(float dt)
        {
            var blockColorCount = blockColors.Length;

            EditorGUILayout.Space(LayoutManager.BLOCK_AREA_PADDING_TOP);
            for (var y = 0; y < blockColorCount; y++)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(LayoutManager.BLOCK_AREA_PADDING_SIDE);

                for (var x = 0; x < blockColorCount; x++)
                {
                    DrawBlock(x, y, dt);
                }

                GUILayout.Space(LayoutManager.BLOCK_AREA_PADDING_SIDE);
                EditorGUILayout.EndHorizontal();
            }
        }

        /// <summary>
        /// ブロックの描画
        /// </summary>
        private static void DrawBlock(int x, int y, float dt)
        {
            GUI.color = GetBlockColor(x, y, dt);
            GUILayout.Button("");
        }

        /// <summary>
        /// ブロックの色を取得
        /// </summary>
        private static Color GetBlockColor(int x, int y, float dt)
        {
            var currentGameState = GameStateManager.CurrentGameState;
            switch (currentGameState)
            {
                case GameState.Clear:
                    if (IsChangedGameState()) animStates[x, y] = x + y;

                    animStates[x, y] += dt * 5;
                    var isColorful = ((int) animStates[x, y]) % 3 == 0;
                    return isColorful ? GetDefaultColor(x, y) : new Color(1, 1, 1, 0);
                
                case GameState.GameOver:
                    if (IsChangedGameState()) animStates[x, y] = 1f + y;

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
        /// ゲーム状態が変わったか
        /// </summary>
        private static bool IsChangedGameState()
        {
            return GameStateManager.CurrentGameState != preGameState;
        }
        
        /// <summary>
        /// ブロックのデフォルト色を取得
        /// </summary>
        private static Color GetDefaultColor(int x, int y)
        {
            var colorIdx = (x + y) % blockColors.Length;
            return blockColors[colorIdx];
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
            var blockColorCount = blockColors.Length;
            var blockRects = new Rect[blockColorCount, blockColorCount];

            var paddingSide = LayoutManager.BLOCK_AREA_PADDING_SIDE;
            var paddingTop = LayoutManager.BLOCK_AREA_PADDING_TOP;
            var blockWidth = (windowSize.x - paddingSide * 2) / blockColorCount;
            var blockHeight = 21;

            for (var y = 0; y < blockColorCount; y++)
            {
                for (var x = 0; x < blockColorCount; x++)
                {
                    if (brokenStates[x, y]) continue;

                    var pos = new Vector2(paddingSide + blockWidth * x - 1, paddingTop + blockHeight * y + 1);
                    var size = new Vector2(blockWidth, blockHeight);
                    blockRects[x, y] = new Rect(pos, size);
                }
            }

            return blockRects;
        }
    }
}