// 事件数据类

using System.Collections.Generic;
using UnityEngine;

public record struct OnMove { }

public record struct PlayerDamageEvent
{
    public int        DamageAmount { get; set; }
    public Vector3    HitPosition  { get; set; }
    public GameObject Attacker     { get; set; }
    public bool       IsCritical   { get; set; }
}

public record struct PlayerHealEvent
{
    public int    HealAmount { get; set; }
    public string HealSource { get; set; }
    public bool   IsOverHeal { get; set; }
}

public record struct PlayerLevelUpEvent
{
    public int               NewLevel        { get; set; }
    public int               TotalExperience { get; set; }
    public List<SkillUnlock> UnlockedSkills  { get; set; }
}

public record struct PlayerDeathEvent
{
    public Vector3    DeathPosition { get; set; }
    public GameObject Killer        { get; set; }
    public string     DeathCause    { get; set; }
}

public record struct GameStateChangedEvent
{
    public GameState NewState           { get; set; }
    public GameState PreviousState      { get; set; }
    public float     TransitionDuration { get; set; }
}

public record struct GamePausedEvent
{
    public bool  IsPaused      { get; set; }
    public float PauseDuration { get; set; }
}

public record struct InputEvent
{
    public string    InputName { get; set; }
    public float     Value     { get; set; }
    public InputType Type      { get; set; }
}

public record struct MouseClickEvent
{
    public Vector2    Position      { get; set; }
    public int        Button        { get; set; }
    public GameObject ClickedObject { get; set; }
}

public enum GameState
{
    Menu,
    Loading,
    Playing,
    Paused,
    GameOver,
    Victory
}

public enum InputType
{
    Button,
    Axis,
    Key
}

public struct SkillUnlock
{
    public string SkillId;
    public int    Level;
}