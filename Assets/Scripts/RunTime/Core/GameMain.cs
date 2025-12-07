using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class GameMain  : MonoBehaviour
{
    public                   Transform        TargetPoint;
    [SerializeField] private TeamInfo         teamInfo;
    public                   GameObject       Player;
    public         List<GameObject> playerModels;
    
    [Inject]public PlayerManager PlayerManager ;
    public InputSystem InputSystem;
    

    private void Awake()
    {
        Debug.Log("GameMain Awake");
        
        
        #region Manager 初始化


        playerModels = LoadPrefabsFromResources("PlayerPrefabs");
        
        Debug.Log($"GameMain playerModels count: {playerModels.Count}");
        
        
        PlayerManager.Initialize(playerModels, TargetPoint); // 初始化 PlayerManager
        
        InputSystem.InputActions.Enable();
        InputSystem.Init();
        
        InputSystem.SwitchCharacterEvent += ctx => PlayerManager.SwitchNextPlayer();

        #endregion
        DontDestroyOnLoad(gameObject);
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
    
    
    public static List<GameObject> LoadPrefabsFromResources(string resourceFolder)
    {
        // 加载该文件夹下所有 GameObject 类型的资源
        GameObject[] array = Resources.LoadAll<GameObject>(resourceFolder);
        return new List<GameObject>(array);
    }
}