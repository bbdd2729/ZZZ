using R3;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("角色配置")]
    [SerializeField] private PlayerName playerName;
    [SerializeField] private int attackLength = 4;
    
    [Header("摄像机配置")]
    [SerializeField] private Vector3 camPosition = new Vector3(0, 2, -5);
    [SerializeField] private Vector3 camRotation = new Vector3(15, 0, 0);
    
    [Header("角色属性")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float maxEnergy = 100f;
    
    // 公共属性
    public PlayerName PlayerName => playerName;
    public int AttackLength => attackLength;
    public Vector3 CamPosition => camPosition;
    public Quaternion CamRotation => Quaternion.Euler(camRotation);
    public float MaxHealth => maxHealth;
    public float MaxEnergy => maxEnergy;
    
    // 私有字段
    private float currentHealth;
    private float currentEnergy;
    private bool isActive = false;
    private CharacterController characterController;
    private Animator animator;
    
    // 状态机
    [HideInInspector] public StateMachine _stateMachine;
    
    // 事件
    public Subject<PlayerEvent> OnPlayerEvent = new Subject<PlayerEvent>();

    // 新增接口方法
    public void SetCharacterData(CharacterData characterData)
    {
        if (characterData == null) return;

        // 设置基础属性
        maxHealth = characterData.maxHealth;
        maxEnergy = characterData.maxEnergy;

        // 设置相机配置
        camPosition = characterData.cameraPosition;
        camRotation = characterData.cameraRotation;

        // 设置战斗配置
        attackLength = characterData.attackComboLength;

        // 应用当前属性
        currentHealth = maxHealth;
        currentEnergy = maxEnergy;
    }

    public void SetPlayerName(PlayerName name)
    {
        playerName = name;
    }

    public void ResetPlayer()
    {
        InitializeStats();
        if (_stateMachine != null)
        {
            _stateMachine.ResetStateMachine();
            _stateMachine.ChangeState<IdleState>();
        }
        gameObject.SetActive(false);
    }

    public bool IsActive() => isActive;
    public bool IsAlive() => currentHealth > 0f;

    private void Awake()
    {
        InitializeComponents();
        InitializeStateMachine();
        InitializeStats();
    }
    
    private void Start()
    {
        SetupEventListeners();
    }
    
    private void Update()
    {
        if (!isActive) return;
        
        UpdateCharacterRotation();
        _stateMachine.Update();
        UpdateStats();
    }
    
    private void InitializeComponents()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        
        if (characterController == null)
        {
            Debug.LogError($"PlayerController {playerName}: 缺少CharacterController组件！");
        }
        
        if (animator == null)
        {
            Debug.LogError($"PlayerController {playerName}: 缺少Animator组件！");
        }
    }
    
    private void InitializeStateMachine()
    {
        _stateMachine = new StateMachine(animator, characterController, this);
        
        // 注册所有状态
        RegisterStates();
        
        // 初始状态将在PlayerManager中设置
    }
    
    private void RegisterStates()
    {
        _stateMachine.RegisterState<IdleState>();
        _stateMachine.RegisterState<WalkState>();
        _stateMachine.RegisterState<AttackState>();
        _stateMachine.RegisterState<AttackEndState>();
        _stateMachine.RegisterState<EvadeBackState>();
        _stateMachine.RegisterState<BigSkillState>();
        _stateMachine.RegisterState<SwitchInState>();
    }
    
    private void InitializeStats()
    {
        currentHealth = maxHealth;
        currentEnergy = maxEnergy;
    }
    
    private void SetupEventListeners()
    {
        // 监听角色切换事件
        GameEvents.OnPlayerSwitched
            .Subscribe(OnPlayerSwitched)
            .AddTo(this);
    }
    
    private void OnPlayerSwitched(PlayerSwitchedEvent evt)
    {
        if (evt.CurrentPlayer == this)
        {
            ActivatePlayer();
        }
        else if (evt.PreviousPlayer == this)
        {
            DeactivatePlayer();
        }
    }
    
    public void ActivatePlayer()
    {
        isActive = true;
        gameObject.SetActive(true);
        
        // 重置状态
        if (_stateMachine != null)
        {
            _stateMachine.ChangeState<SwitchInState>();
        }
        
        OnPlayerEvent.OnNext(new PlayerEvent 
        { 
            Type = PlayerEvent.EventType.Activated,
            Player = this,
            Time = Time.time
        });
    }
    
    public void DeactivatePlayer()
    {
        isActive = false;
        
        // 切换到切出状态
        if (_stateMachine != null)
        {
            _stateMachine.ChangeState<SwitchOutState>();
        }
        
        OnPlayerEvent.OnNext(new PlayerEvent 
        { 
            Type = PlayerEvent.EventType.Deactivated,
            Player = this,
            Time = Time.time
        });
        
        // 延迟禁用游戏对象
        Observable.Timer(TimeSpan.FromSeconds(0.5f))
            .Subscribe(_ => gameObject.SetActive(false))
            .AddTo(this);
    }
    
    private void UpdateCharacterRotation()
    {
        if (Camera.main != null)
        {
            float targetAngle = Camera.main.transform.eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }
    
    private void UpdateStats()
    {
        // 能量恢复
        if (currentEnergy < maxEnergy)
        {
            currentEnergy += 10f * Time.deltaTime; // 每秒恢复10点能量
            currentEnergy = Mathf.Min(currentEnergy, maxEnergy);
        }
    }
    
    // 公共方法
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0f);
        
        OnPlayerEvent.OnNext(new PlayerEvent 
        { 
            Type = PlayerEvent.EventType.Damaged,
            Player = this,
            Value = damage,
            Time = Time.time
        });
        
        if (currentHealth <= 0f)
        {
            Die();
        }
    }
    
    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        
        OnPlayerEvent.OnNext(new PlayerEvent 
        { 
            Type = PlayerEvent.EventType.Healed,
            Player = this,
            Value = amount,
            Time = Time.time
        });
    }
    
    public bool UseEnergy(float amount)
    {
        if (currentEnergy >= amount)
        {
            currentEnergy -= amount;
            return true;
        }
        return false;
    }
    
    public void AddEnergy(float amount)
    {
        currentEnergy += amount;
        currentEnergy = Mathf.Min(currentEnergy, maxEnergy);
    }
    
    private void Die()
    {
        if (_stateMachine != null)
        {
            _stateMachine.ChangeState<DeathState>();
        }
        
        OnPlayerEvent.OnNext(new PlayerEvent 
        { 
            Type = PlayerEvent.EventType.Death,
            Player = this,
            Time = Time.time
        });
        
        // 触发游戏事件
        GameEvents.OnCombatEvent.OnNext(new CombatEvent 
        { 
            EventType = CombatEvent.CombatEventType.Death,
            Source = this,
            EventTime = Time.time
        });
    }
    
    // 获取器
    public float GetCurrentHealth()
    {
        return currentHealth;
    }
    
    public float GetCurrentEnergy()
    {
        return currentEnergy;
    }
    
    public bool IsActive()
    {
        return isActive;
    }
    
    public bool IsAlive()
    {
        return currentHealth > 0f;
    }
    
    // 编辑器调试
    private void OnValidate()
    {
        if (Application.isPlaying && _stateMachine != null)
        {
            // 在编辑器中验证时更新摄像机配置
            camPosition = Vector3.ClampMagnitude(camPosition, 10f);
            camRotation.x = Mathf.Clamp(camRotation.x, -89f, 89f);
        }
    }
}

// 玩家事件数据
public class PlayerEvent
{
    public enum EventType
    {
        Activated,
        Deactivated,
        Damaged,
        Healed,
        Death
    }
    
    public EventType Type { get; set; }
    public PlayerController Player { get; set; }
    public float Value { get; set; }
    public float Time { get; set; }
}