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
            go.SetActive(i == 0);
            DebugX.Instance.Log($"实例化玩家：{playerInfo.PlayerName}");

            // 关键：把 PlayerController 交给 Manager
            PlayerManager.Instance.AddPlayer(go.GetComponent<PlayerController>());


            if (i == 0)
                PlayerManager.Instance.CurrentPlayer = go.GetComponent<PlayerController>();
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