using System.Collections.Generic;
using UnityEngine;

public class GameMain  : MonoBehaviour
{
    public                   Transform        TargetPoint;
    [SerializeField] private TeamInfo         teamInfo;
    public                   GameObject       Player;
    private readonly         List<GameObject> playerModels = new List<GameObject>(3);

    
    private void Awake()
    {

        #region Manager 初始化

        PlayerManager.Instance.Init();  // 初始化 PlayerManager
        InputSystem.Instance.InputActions.Enable();
        InputSystem.Instance.Init();
        UIManager.Instance.Init();
        #endregion

        //todo 场景加载器
        //PlayerManager.Instance.LoadPlayer();

        for (int i = 0; i < teamInfo.PlayerInfoList.Length; i++)
        {
            PlayerInfo playerInfo = teamInfo.PlayerInfoList[i];
            if (playerInfo == null || playerInfo.Prefab == null) continue;

            // 实例化
            GameObject go = Instantiate(playerInfo.Prefab, TargetPoint.position, Quaternion.identity);
            go.name = playerInfo.PlayerName;
            
            // 只激活第一个角色
            go.SetActive(i == 0);
            DebugX.Instance.Log($"实例化玩家：{playerInfo.PlayerName}");

            // 关键：把 PlayerController 交给 Manager
            PlayerController controller = go.GetComponent<PlayerController>();
            PlayerManager.Instance.AddPlayer(controller);

            // 确保非激活角色的组件被正确禁用
            if (i != 0)
            {
                controller.SetInputActive(false);
            }

            if (i == 0)
                PlayerManager.Instance.CurrentPlayer = controller;
        }

    }

    private void Start()
    {

    }

    private void Update()
    {

    }

    private void FixedUpdate()
    {

    }

    private void LateUpdate()
    {

    }
}