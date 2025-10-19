using UnityEngine;

public class DebugX : SingletonBase<DebugX>
{
    public DebugX()
    {
        Debug.Log("Debug X初始化成功");
    }

    public void Log(string message)
    {
        Debug.Log(message);
    }

    public void LogWarning(string message)
    {
        Debug.LogWarning(message);
    }

    public void LogError(string message)
    {
        Debug.LogError(message);
    }
}