using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class BreakoutWindow : EditorWindow
    {
        private static float preTime;
        private static float dt;
        
        [MenuItem("Window/BreakoutWindow")]
        private static void ShowWindow()
        {
            preTime = (float) EditorApplication.timeSinceStartup;
            
            var window = GetWindow<BreakoutWindow>();
            window.titleContent = new GUIContent("Breakout");
            
            GameInitializer.Initialize();
        }
        
        private void OnGUI()
        {
            // 経過時間の更新
            var currentTime = (float) EditorApplication.timeSinceStartup;
            dt = currentTime - preTime;
            preTime = currentTime;
            
            LayoutManager.CheckWindowSize(position);
            if (GameStateManager.IsPause)
            {
                MessageShower.ShowTopMessage();
                return;
            }

            var windowSize = position.size;
            
            BlockUpdater.UpdateBlocks(dt);
            BarUpdater.UpdateBar(windowSize, dt);
            BallUpdater.UpdateBall(windowSize, dt);
            WindowEdgeDrawer.DrawWindowFrame(windowSize, dt);
            MessageShower.ShowCenterMessage();
            
            GameInitializer.DrawInitializeButton(windowSize);
            GameInitializer.CheckPlayGame();
        }

        private void Update()
        {
            Repaint();
        }
    }
}
