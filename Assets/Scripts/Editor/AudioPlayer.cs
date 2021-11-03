using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    /// <summary>
    /// Audioの再生
    /// </summary>
    public static class AudioPlayer
    {
        private static Dictionary<ClipName, AudioClip> clips = new Dictionary<ClipName, AudioClip>();
        private static Dictionary<ClipName, string> clipPaths = new Dictionary<ClipName, string>()
        {
            {ClipName.CollisionFrame, "Assets/Audios/collision_frame.mp3"},
            {ClipName.CollisionBlock, "Assets/Audios/collision_block.mp3"},
            {ClipName.CollisionBar, "Assets/Audios/collision_bar.mp3"},
            {ClipName.GameClear, "Assets/Audios/clear.mp3"},
            {ClipName.GameOver, "Assets/Audios/gameover.mp3"},
        };

        public static void Initialize()
        {
            LoadClips();
        }
        
        /// <summary>
        /// AudioClipをロード
        /// </summary>
        private static void LoadClips()
        {
            foreach (var clipPathPair in clipPaths)
            {
                var clipName = clipPathPair.Key;
                if(clips.ContainsKey(clipName)) continue;
                
                var clipPath = clipPathPair.Value;
                
                var clip = (AudioClip)AssetDatabase.LoadAssetAtPath(clipPath, typeof(AudioClip));
                
                if(clip == null) continue;
                clips.Add(clipName, clip);
            }
        }

        /// <summary>
        /// Audioを再生
        /// </summary>
        public static void Play(ClipName clipName)
        {
            if(!clips.ContainsKey(clipName)) return;
            PlayClip(clips[clipName]);
        }
        
        /// <summary>
        /// AudioClipをリフレクションで再生する
        /// </summary>
        private static void PlayClip(AudioClip clip)
        {
            var unityEditorAssembly = typeof(AudioImporter).Assembly;
            var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(
                    "PlayPreviewClip",
                    BindingFlags.Static | BindingFlags.Public,
                    null,
                    new Type[] {typeof(AudioClip), typeof(int), typeof(bool)},
                    null
                );
            method?.Invoke(null, new object[] {clip, 0, false});
        }

        public enum ClipName
        {
            CollisionFrame,
            CollisionBlock,
            CollisionBar,
            GameClear,
            GameOver,
        }
    }
}