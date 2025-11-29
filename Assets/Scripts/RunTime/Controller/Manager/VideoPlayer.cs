using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Video;

public class IntroPlayer
    {
        private readonly GameConfig _config;
        public IntroPlayer(GameConfig config)
        { _config = config; }
        
        public async UniTask PlayVideoAsync()
        {
            // 新建一个 GO 挂 VideoPlayer
            var go      = new GameObject("[IntroPlayer]");
            var vp      = go.AddComponent<VideoPlayer>();
            vp.playOnAwake = false;
            vp.renderMode  = VideoRenderMode.CameraNearPlane;
            vp.url         = System.IO.Path.Combine(Application.streamingAssetsPath, _config.videoName);

            // 让视频播完自动销毁
            var tcs = new UniTaskCompletionSource();
            vp.loopPointReached += _ => tcs.TrySetResult();
            vp.Play();

            await tcs.Task;          // 等视频播完
            Object.Destroy(go);
        }
        
        
        
        
        
    }
