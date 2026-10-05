using UnityEngine;

// Concrete player adapter keeps the original component and serialized field names.
public class StateMachine : StateMachineBase<Context, State, StateFactory>
{
    public Context currentContext;
    public StatesList playerStates;
    public _States initalState;
    [HideInInspector] public _States currentEnumState;
    [HideInInspector] public State mainState;
    [HideInInspector] public State currentParallelState;

    protected override void ContextAwake()
    {
        if (currentContext == null) currentContext = GetComponent<Context>();
        if (currentContext == null) throw new System.InvalidOperationException("StateMachine requires a Context.");
        currentContext.ContextStart();
    }

    protected override void Initialize()
    {
        factory = new StateFactory(this, playerStates);
        currentState = factory.GetState(initalState);
        currentState.OnEnter();
    }

    protected override void ContextUpdate() => currentContext.ContextUpdate();

    private void OnDestroy()
    {
        currentState?.OnExit();
        factory?.Dispose();
    }
}
