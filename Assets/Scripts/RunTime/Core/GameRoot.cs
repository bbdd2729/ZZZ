using VContainer;
using VContainer.Unity;

public class GameRoot : IStartable
    {
        
        
        [Inject]private readonly IntroPlayer _intro;
        [Inject]private readonly SceneLoader _scene;
        [Inject]private readonly GameConfig _config;
        
        
        public GameRoot(IntroPlayer intro, SceneLoader scene)
        {
            _intro = intro;
            _scene = scene;
        }
        public async void Start()
        {
            // 1. 同时开始播放视频 + 异步加载场景
            var introTask   = _intro.PlayVideoAsync();
            var sceneTask   = _scene.LoadSceneAsync(_config.startSceneName);

            // 2. 等视频播完（如果场景还没好就继续等）
            await introTask;

            // 3. 等场景加载完
            await sceneTask;
            
        }
    }
