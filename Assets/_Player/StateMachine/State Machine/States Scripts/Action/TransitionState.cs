using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Transition State", menuName = "States List/Transition State")]
public class TransitionState : State
{
    public _States transitionToState;
    public override void CheckSwitchState()
    {
        base.CheckSwitchState();
        if (!IsCurrentState) return;
        if (Time.time - enterTime >= duration)
        {
            SwitchState(factory.GetState(transitionToState));
            return;
        }
    }

    public override void OnEnter()
    {
        base.OnEnter();
        Vector2 newVelocity = new Vector3(0, currentContext.Rb.velocity.y, 0);
        currentContext.Rb.velocity = newVelocity;
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
    }

    public override void OnLateUpdate()
    {
        base.OnLateUpdate();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

    }
}
