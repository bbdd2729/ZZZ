using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Game/Config")]
public class GameConfig : ScriptableObject
    {
        public string gameName    = "Demo";
        public int    gameVersion = 1;
        public string videoPath   = "Assets/Videos/";
        public string videoName   = "Demo.mp4";
        public string startSceneName = "Start";
    }
