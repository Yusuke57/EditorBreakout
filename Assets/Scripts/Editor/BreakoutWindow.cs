using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class BreakoutWindow : EditorWindow
    {
        private static float preTime;
        
        [MenuItem("Window/BreakoutWindow")]
        private static void ShowWindow()
        {
            preTime = (float) EditorApplication.timeSinceStartup;
            
            var window = GetWindow<BreakoutWindow>();
            window.titleContent = new GUIContent("Breakout");
            
            GameInitializer.Initialize();
        }
        
        /// <summary>
        /// 描画
        /// </summary>
        private void OnGUI()
        {
            if (!LayoutManager.CheckWindowSize(position))
            {
                MessageShower.ShowTopMessage();
                return;
            }

            BlockUpdater.DrawBlocks();
            BarUpdater.DrawBar();
            BallUpdater.DrawBall();
            WindowEdgeUpdater.DrawWindowFrame();
            MessageShower.ShowCenterMessage();
            
            GameInitializer.DrawInitializeButton(position.size);
            GameInitializer.CheckPlayGame();
        }

        /// <summary>
        /// ロジック
        /// </summary>
        private void Update()
        {
            if (!LayoutManager.CheckWindowSize(position))
            {
                MessageShower.ShowTopMessage();
                return;
            }
            
            var windowSize = position.size;
            var currentTime = (float) EditorApplication.timeSinceStartup;
            var dt = currentTime - preTime;
            preTime = currentTime;
            
            BlockUpdater.UpdateBlocks(dt);
            BarUpdater.UpdateBar(windowSize, dt);
            BallUpdater.UpdateBall(windowSize, dt);
            WindowEdgeUpdater.UpdateWindowEdges(windowSize, dt);
            
            Repaint();
        }
    }
}
