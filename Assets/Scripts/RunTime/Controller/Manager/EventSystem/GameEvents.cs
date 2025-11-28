using R3;

public static class GameEvents
{
    //public static readonly Subject<PlayerDamageEvent>  OnPlayerDamaged = new();
    //public static readonly Subject<PlayerHealEvent>    OnPlayerHealed  = new();
    //public static readonly Subject<PlayerLevelUpEvent> OnPlayerLevelUp = new();
    //public static readonly Subject<PlayerDeathEvent>   OnPlayerDeath   = new();


    // 游戏状态事件
    //public static readonly Subject<GameStateChangedEvent> OnGameStateChanged = new();

    //public static readonly Subject<SceneLoadedEvent>      OnSceneLoaded    =   new();
    //public static readonly Subject<GamePausedEvent> OnGamePaused = new();

    // 输入事件
    public static readonly Subject<InputEvent>      OnInput      = new();
    //public static readonly Subject<MouseClickEvent> OnMouseClick = new();

    // UI事件
    //public static readonly Subject<Button.ButtonClickedEvent> OnButtonClicked = new();
    //public static readonly Subject<SliderChangedEvent>      OnSliderChanged =   new();
}