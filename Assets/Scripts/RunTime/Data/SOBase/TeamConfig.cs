using UnityEngine;

[CreateAssetMenu(menuName = "Game/TeamConfig")]
public class TeamConfig : ScriptableObject
{
    public PlayerConfig[] PlayerConfigList = new PlayerConfig[3]; // 玩家信息列表

    public int Length
    {
        get => PlayerConfigList.Length;
    } // 获取玩家数量

    public PlayerConfig GetMember(int index)
    {
        return PlayerConfigList[index];
        // 获取指定索引的玩家信息
    }
}