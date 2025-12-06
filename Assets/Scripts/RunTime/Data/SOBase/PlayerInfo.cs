using UnityEngine;

[CreateAssetMenu(menuName = "Game/PlayerInfo")]
public class PlayerConfig : ScriptableObject
{
    [Header("仅运行时读，不在资产里保存")]
    public GameObject RuntimeModel; // 加载后缓存，方便别处用

    public GameObject Prefab;         // 拖预制体（或填 Addressable Key）
    public int        MaxHP;
    public string     PlayerName;
    public int        AttackLength = 4;
}