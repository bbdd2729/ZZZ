using UnityEngine;

[CreateAssetMenu(menuName = "Game/TeamInfo")]
public class TeamInfo : ScriptableObject
{
    public PlayerConfig[] PlayerInfoList = new PlayerConfig[3];   // 在 Inspector 里拖 3 个 SO

    public PlayerConfig GetMember(int index) => PlayerInfoList[index];
    public int         Length               => PlayerInfoList.Length;
}