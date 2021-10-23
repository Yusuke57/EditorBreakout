using UnityEditor;

namespace Editor
{
    public class BreakoutWindow : EditorWindow
    {
        private static float preTime;
        
        [MenuItem("Tools/BreakoutWindow")]
        private static void ShowWindow()
        {
            preTime = (float) EditorApplication.timeSinceStartup;
            GetWindow<BreakoutWindow>();
        }
        
        private void OnGUI()
        {
            // 経過時間の更新
            var currentTime = (float) EditorApplication.timeSinceStartup;
            var dt = currentTime - preTime;
            preTime = currentTime;
            
            LayoutManager.CheckWindowSize(position);
            if (GameStateManager.IsPause)
            {
                MessageShower.ShowTopMessage();
                return;
            }
            
            BlockUpdater.UpdateBlocks(dt);
            BarUpdater.UpdateBar(position.size, dt);
            BallUpdater.UpdateBall(position.size, dt);
            WindowEdgeDrawer.DrawWindowFrame(position.size, dt);
            MessageShower.ShowCenterMessage();
            
            GameInitializer.DrawInitializeButton(position.size);
            GameInitializer.CheckPlayGame();
            
            Repaint();
        }
    }
}
