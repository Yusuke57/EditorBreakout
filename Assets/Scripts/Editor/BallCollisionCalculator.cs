using UnityEngine;

namespace Editor
{
    /// <summary>
    /// ボールの衝突を計算
    /// </summary>
    public static class BallCollisionCalculator
    {
        /// <summary>
        /// バーで反射
        /// </summary>
        public static Vector2 ReflectBar(Vector2 prePos, Vector2 nextPos, float ballRadius, ref Vector2 moveDir)
        {
            var barRect = BarUpdater.GetBarRect();
            var reflectPosY = barRect.yMin - ballRadius;
            if (!barRect.Contains(prePos) && !barRect.Contains(nextPos))
            {
                if (!(prePos.y < reflectPosY && nextPos.y >= reflectPosY))
                {
                    return nextPos;
                }

                var vec = nextPos - prePos;
                var collidablePos = prePos + vec * ((reflectPosY - prePos.y) / vec.y);
                if (collidablePos.x < barRect.xMin || collidablePos.x > barRect.xMax)
                {
                    return nextPos;
                }
            }

            nextPos.y = reflectPosY * 2 - nextPos.y;
            moveDir.y *= -1;
            moveDir = GetModifiedBallDir(moveDir);

            BarUpdater.OnCollisionBall();
            AudioPlayer.Play(AudioPlayer.ClipName.CollisionBar);
            
            return nextPos;
        }

        /// <summary>
        /// バーの移動速度によってボールの方向に修正を加える
        /// </summary>
        private static Vector2 GetModifiedBallDir(Vector2 originDir)
        {
            var barMoveDiff = BarUpdater.GetBarMoveDiff();
            var ballAdditiveAngle = -barMoveDiff * 5;
            var modifiedDir = Quaternion.Euler(0, 0, ballAdditiveAngle) * originDir;
            var angle = Vector2.SignedAngle(Vector2.down, modifiedDir);
            if (Mathf.Abs(angle) > 60)
            {
                modifiedDir = Quaternion.Euler(0, 0, 60 * Mathf.Sign(angle)) * Vector2.down;
            }

            return modifiedDir;
        }

        /// <summary>
        /// ブロックで反射
        /// </summary>
        public static Vector2 ReflectBlocks(Vector2 prePos, Vector2 nextPos, Vector2 windowSize, float ballRadius, ref Vector2 moveDir)
        {
            var blockRects = BlockUpdater.GetBlockRects(windowSize);
            for (var y = 0; y < blockRects.GetLength(0); y++)
            {
                for (var x = 0; x < blockRects.GetLength(1); x++)
                {
                    if(BlockUpdater.IsBroken(x, y)) continue;
                    
                    var blockRect = blockRects[x, y];
                    blockRect = new Rect(blockRect.x - ballRadius, blockRect.y - ballRadius,
                        blockRect.width + ballRadius * 2, blockRect.height + ballRadius * 2);
                    
                    var intersection = IntersectionCalculator.SearchIntersection(blockRect, prePos, nextPos);
                    if(intersection == Direction.None) continue;

                    if (intersection == Direction.Top || intersection == Direction.Bottom)
                    {
                        moveDir.y *= -1;
                    } 
                    else if (intersection == Direction.Left || intersection == Direction.Right)
                    {
                        moveDir.x *= -1;
                    }

                    BlockUpdater.Break(x, y);
                    AudioPlayer.Play(AudioPlayer.ClipName.CollisionBlock);
                    
                    return prePos;
                }
            }

            return nextPos;
        }

        /// <summary>
        /// ウィンドウ枠で反射
        /// </summary>
        public static Vector2 ReflectWindowFrame(Vector2 nextPos, Vector2 windowSize, float ballRadius, ref Vector2 moveDir)
        {
            if (nextPos.x < ballRadius)
            {
                nextPos.x = ballRadius * 2 - nextPos.x;
                moveDir.x *= -1;
                WindowEdgeDrawer.OnCollisionBall(WindowEdgeDrawer.WindowEdge.Left);
                AudioPlayer.Play(AudioPlayer.ClipName.CollisionFrame);
            }
            
            if (nextPos.x > windowSize.x - ballRadius)
            {
                nextPos.x = (windowSize.x - ballRadius) * 2 - nextPos.x;
                moveDir.x *= -1;
                WindowEdgeDrawer.OnCollisionBall(WindowEdgeDrawer.WindowEdge.Right);
                AudioPlayer.Play(AudioPlayer.ClipName.CollisionFrame);
            }

            if (nextPos.y < ballRadius)
            {
                nextPos.y = ballRadius * 2 - nextPos.y;
                moveDir.y *= -1;
                WindowEdgeDrawer.OnCollisionBall(WindowEdgeDrawer.WindowEdge.Top);
                AudioPlayer.Play(AudioPlayer.ClipName.CollisionFrame);
            }

            if (nextPos.y > windowSize.y - ballRadius)
            {
                nextPos.y = (windowSize.y - ballRadius) * 2 - nextPos.y;
                moveDir = Vector2.zero;
                WindowEdgeDrawer.OnCollisionBall(WindowEdgeDrawer.WindowEdge.Bottom);
                AudioPlayer.Play(AudioPlayer.ClipName.CollisionFrame);
                
                GameStateManager.OnGameOver();
            }

            return nextPos;
        }
    }
}