using System.Collections.Generic;
using UnityEngine;

public class GameMain  : MonoBehaviour
{
    
    public  Transform PlayerGenerateTargetPoint; // 玩家生成目标点
    
    [SerializeField] private TeamConfig  teamInfo; // 队伍信息SO
    
    public List<GameObject> playerModels = new List<GameObject>(3); // 玩家模型列表
    
    
    private void Awake()
    {
        #region Manager 初始化

        PlayerManager.Instance.Init();  // 初始化 PlayerManager
        InputSystem.Instance.Init();  // 初始化InputSystem
        InputSystem.Instance.InputActions.Enable();  // 开启输入响应
        SceneLoader.Instance.Init();  // 初始化 SceneLoader
        
        #endregion

        LoadPlayers();
    }
    
    private void LoadPlayers()
    { 
        for (int i = 0; i < teamInfo.PlayerConfigList.Length; i++)
        {
            PlayerConfig playerConfig = teamInfo.PlayerConfigList[i];
            if (playerConfig == null || playerConfig.PlayerPrefab == null) continue;
            GameObject go = Instantiate(playerConfig.PlayerPrefab, PlayerGenerateTargetPoint.position, Quaternion.identity);
            
            go.name = playerConfig.PlayerName;
            go.SetActive(i == 0);// 只激活第一个角色
            Debug.Log($"实例化玩家：{playerConfig.PlayerName}");
            
            playerModels.Add(go);
            PlayerManager.Instance.PlayerInstances.Add(go);
            PlayerManager.Instance.PlayerControllers.Add(go.GetComponent<PlayerController>());
        }
    }
}