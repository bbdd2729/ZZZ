using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : ISceneLoader
    { 
        public static SceneLoader Instance { get; private set;} = new SceneLoader();
        
        
        
        public void Init()
        {
            return;
        }
        
        
        public SceneLoader()
        {
            Instance = this;
        }
        
        public async UniTask LoadSceneAsync(string sceneName)
        {
            await SceneManager.LoadSceneAsync(sceneName);
        }
        
        
    }
