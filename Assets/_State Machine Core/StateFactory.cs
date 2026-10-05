using UnityEngine;

public class StateFactory : StateFactoryBase<StateMachine, Context, _States, State, StatesList, StatesList.StateEntry>
{
    public StateFactory(StateMachine machine, StatesList config) : base(machine, config) { }
    protected override State CreateState(State asset) => Object.Instantiate(asset);
    protected override void InitializeState(State state) => state.Initialize(stateMachine, this, stateMachine.currentContext);
}
