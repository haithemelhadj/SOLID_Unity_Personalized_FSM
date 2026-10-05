using UnityEngine;

// Animation is a player dependency, not a dependency of the reusable core.
public abstract class State : StateBase<StateMachine, StateFactory, Context, State>
{
    public override void OnEnter()
    {
        base.OnEnter();
        if (stateMachine.logStateChange) Debug.Log("Enter State: " + this);
        if (currentContext.animatorController != null)
            currentContext.animatorController.PlayAnimation(animationName, 0f);
    }

    public virtual void OnTriggerStay2D() { }
}
