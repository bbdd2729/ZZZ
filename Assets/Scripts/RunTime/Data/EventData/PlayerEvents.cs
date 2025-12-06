using System;

[Serializable]
public class PlayerSwitchedEvent
{
    public PlayerController PreviousPlayer { get; set; }
    public PlayerController CurrentPlayer { get; set; }
    public float SwitchStartTime { get; set; }
    public float SwitchEndTime { get; set; }
    public float SwitchDuration => SwitchEndTime - SwitchStartTime;
}

[Serializable]
public class PlayerSpawnedEvent
{
    public PlayerName PlayerName { get; set; }
    public PlayerController PlayerController { get; set; }
    public float SpawnTime { get; set; }
}