using UnityEngine;

[CreateAssetMenu(menuName = "Game/TeamInfo")]
public class TeamInfo : ScriptableObject
{
    public PlayerInfo[] PlayerInfoList = new PlayerInfo[3];   // 在 Inspector 里拖 3 个 SO

    public PlayerInfo GetMember(int index) => PlayerInfoList[index];
    public int         Length               => PlayerInfoList.Length;
}