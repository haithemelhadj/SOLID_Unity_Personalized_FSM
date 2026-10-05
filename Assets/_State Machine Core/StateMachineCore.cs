using UnityEngine;

public interface IStateMachine<TState>
{
    TState CurrentState { get; set; }
    string CurrentStateName { get; set; }
}

public interface IStateLifecycle
{
    void OnEnter();
    void OnExit();
    void OnUpdate();
    void OnFixedUpdate();
    void OnLateUpdate();
    void OnTriggerEnter2D(Collider2D other);
    void OnTriggerStay2D(Collider2D other);
    void OnTriggerExit2D(Collider2D other);
    void OnCollisionEnter2D(Collision2D collision);
    void OnCollisionStay2D(Collision2D collision);
    void OnCollisionExit2D(Collision2D collision);
}

public interface IStateEntry<TEnum, TState>
{
    TEnum StateKey { get; }
    TState StateAsset { get; }
}

public interface IStateListConfig<TEnum, TState, TEntry>
    where TEntry : class, IStateEntry<TEnum, TState>
{
    System.Collections.Generic.List<TEntry> Entries { get; }
}

public abstract class StateMachineBase<TContext, TState, TFactory> : MonoBehaviour, IStateMachine<TState>
    where TState : class, IStateLifecycle
{
    [Header("------------------GENERAL-----------------")]
    public bool logStateChange = false;
    public bool logCombat = false;
    public bool logDamage = false;

    [Header("Gizmos")]
    public bool showGizmos;



    [Header("-----DEBUGGING-----")]
    public string currentStateName;

    [Header("-----STATE MACHINE-----")]
    [SerializeField] public TFactory factory;

    [HideInInspector] public TState currentState;

    public TState CurrentState
    {
        get => currentState;
        set => currentState = value;
    }

    public string CurrentStateName
    {
        get => currentStateName;
        set => currentStateName = value;
    }

    protected virtual void ContextAwake()
    {

    }

    protected virtual void ContextStart()
    {

    }

    protected virtual void ContextUpdate()
    {

    }

    protected abstract void Initialize();

    protected virtual void Awake()
    {
        ContextAwake();
        Initialize();
    }

    protected virtual void Start()
    {
        ContextStart();
    }

    private void Update()
    {
        ContextUpdate();
        if (currentState != null)
            currentState.OnUpdate();
    }

    private void FixedUpdate()
    {
        if (currentState != null)
            currentState.OnFixedUpdate();
    }

    private void LateUpdate()
    {
        if (currentState != null)
            currentState.OnLateUpdate();
    }

    #region 2D Collision Detection
    private void OnTriggerEnter2D(Collider2D other)
    {
        currentState?.OnTriggerEnter2D(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        currentState?.OnTriggerStay2D(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        currentState?.OnTriggerExit2D(other);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        currentState?.OnCollisionEnter2D(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        currentState?.OnCollisionStay2D(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        currentState?.OnCollisionExit2D(collision);
    }
    #endregion
}

public abstract class StateFactoryBase<TMachine, TContext, TEnum, TState, TConfig, TEntry>
    where TMachine : MonoBehaviour, IStateMachine<TState>
    where TState : class, IStateLifecycle
    where TConfig : ScriptableObject, IStateListConfig<TEnum, TState, TEntry>
    where TEntry : class, IStateEntry<TEnum, TState>
{
    protected readonly TMachine stateMachine;
    protected readonly System.Collections.Generic.Dictionary<TEnum, TState> states =
        new System.Collections.Generic.Dictionary<TEnum, TState>();

    protected StateFactoryBase(TMachine currentContext, TConfig config)
    {
        stateMachine = currentContext;

        if (config == null) throw new System.ArgumentNullException(nameof(config));
        if (config.Entries == null) throw new System.ArgumentException("State entries are missing.", nameof(config));
        if (config.Entries.Count <= 0)
        {
            Debug.LogWarning("config empty");
            return;
        }

        foreach (TEntry entry in config.Entries)
        {
            if (entry == null) continue;
            if (!states.ContainsKey(entry.StateKey) && entry.StateAsset != null)
            {
                TState instance = CreateState(entry.StateAsset);
                InitializeState(instance);
                states.Add(entry.StateKey, instance);
            }
        }
    }

    protected abstract TState CreateState(TState stateAsset);
    protected abstract void InitializeState(TState state);

    public void Dispose()
    {
        foreach (TState state in states.Values)
        {
            if (state is UnityEngine.Object instance)
            {
                if (Application.isPlaying) UnityEngine.Object.Destroy(instance);
                else UnityEngine.Object.DestroyImmediate(instance);
            }
        }
        states.Clear();
    }

    public TState GetState(TEnum state)
    {
        if (states.TryGetValue(state, out TState result)) return result;
        throw new System.InvalidOperationException($"State {state} is not configured on {stateMachine.name}.");
    }
}

public abstract class StateBase<TMachine, TFactory, TContext, TState> : ScriptableObject, IStateLifecycle
    where TMachine : MonoBehaviour, IStateMachine<TState>
    where TState : class, IStateLifecycle
{
    protected TMachine stateMachine;
    protected TFactory factory;
    protected TContext currentContext;

    public float duration = 1f;
    public string animationName;

    protected float enterTime { get; set; }
    protected float updateTime { get; set; }
    protected float fixedTime { get; set; }
    protected float lateTime { get; set; }
    protected float exitTime { get; set; }

    public void Initialize(TMachine machine, TFactory stateFactory, TContext context)
    {
        stateMachine = machine;
        factory = stateFactory;
        currentContext = context;
    }

    public virtual void OnEnter()
    {
        stateMachine.CurrentStateName = this.ToString();
        enterTime = Time.time;
        updateTime = fixedTime = lateTime = 0f;
    }

    public virtual void OnUpdate()
    {
        updateTime += Time.deltaTime;
    }

    public virtual void OnFixedUpdate()
    {
        fixedTime += Time.fixedDeltaTime;
    }

    public virtual void OnLateUpdate()
    {
        lateTime += Time.deltaTime;
        CheckSwitchState();
    }

    public virtual void OnExit()
    {
        exitTime = Time.time;
    }

    public virtual void CheckSwitchState() { }

    protected bool IsCurrentState => ReferenceEquals(stateMachine.CurrentState, this);

    protected void SwitchState(TState newState)
    {
        if (!IsCurrentState || newState == null || ReferenceEquals(newState, this)) return;
        OnExit();
        stateMachine.CurrentState = newState;
        newState.OnEnter();
    }

    #region 2D Collision Detection
    public virtual void OnTriggerEnter2D(Collider2D other) { }
    public virtual void OnTriggerStay2D(Collider2D other) { }
    public virtual void OnTriggerExit2D(Collider2D other) { }

    public virtual void OnCollisionEnter2D(Collision2D collision) { }
    public virtual void OnCollisionStay2D(Collision2D collision) { }
    public virtual void OnCollisionExit2D(Collision2D collision) { }
    #endregion

    #region Passthrough Methods
    protected static new void Destroy(UnityEngine.Object obj)
    {
        UnityEngine.Object.Destroy(obj);
    }

    protected T GetComponent<T>() where T : Component
    {
        return stateMachine.GetComponent<T>();
    }

    protected Component GetComponent(System.Type type)
    {
        return stateMachine.GetComponent(type);
    }

    protected Component GetComponent(string type)
    {
        return stateMachine.GetComponent(type);
    }
    #endregion
}