using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Parry State", menuName = "States List/Player/Parry State")]
public class ParryState : TransitionState
{
    public override void CheckSwitchState()
    {
        if(!currentContext.defendInput)
        {
            SwitchState(factory.GetState(_States.Grounded));
            return;

        }
        base.CheckSwitchState();
        if (!IsCurrentState) return;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        currentContext.parryHitBox.SetActive(true);
    }

    public override void OnExit()
    {
        base.OnExit();
        currentContext.parryHitBox.SetActive(false);
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
